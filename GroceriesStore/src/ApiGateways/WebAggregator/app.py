from flask import Flask, request, jsonify
from pip._vendor import requests
import json
from tenacity import retry, stop_after_attempt, wait_fixed

app = Flask(__name__)

@retry(stop=stop_after_attempt(3), wait=wait_fixed(2))
def make_request_to_basket(url, data, headers):
    return requests.post(url, data=data, headers=headers)


@app.route('/api/v1/basket', methods=['POST'])
def update_all_basket():
    data = request.get_json()

    if data is None or not data.get("Items"):
        return jsonify({"error": "Need to pass at least one basket line"}), 400

    product_ids = [item["ProductId"] for item in data["Items"]]
    
  
    
    #catalog_item_response = requests.get(f'http://localhost:5222/api/v1/Catalog/items?ids={",".join(map(str, product_ids))}')
    catalog_item_response = requests.get(f'http://catalog-api/api/v1/Catalog/items?ids={",".join(map(str, product_ids))}')

    if catalog_item_response.status_code == 200:
        content_string = catalog_item_response.text
        data["catalogItems"] = json.loads(content_string)

        basket_content = json.dumps(data)
        headers = {'Content-Type': 'application/json'}
        basket_response = requests.post('http://localhost:5221/api/v1/Direct', data=basket_content, headers=headers)
        #basket_response = requests.post('http://basket-api/api/v1/Direct', data=basket_content, headers=headers)

        if basket_response.status_code == 200:
            return jsonify(basket_response.json()), 200

    return '', 500

@app.route('/api/v1/basket/items', methods=['PUT'])
def update_quantities():
    data = request.get_json()

    if not data or not data.get("Updates"):
        return jsonify({"error": "No updates sent"}), 400

    basket_content = json.dumps(data)
    headers = {'Content-Type': 'application/json'}
    basket_response = requests.put('http://localhost:5221/api/v1/Direct', data=basket_content, headers=headers)
    #basket_response = requests.put('http://basket-api/api/v1/Direct/items', data=basket_content, headers=headers)

    if basket_response.status_code == 200:
        return jsonify(basket_response.json()), 200

    return '', 500

@app.route('/api/v1/basket/items', methods=['POST'])
def add_basket_item():
    data = request.get_json()

    if data is None:
        return jsonify({"error": "Invalid payload"}), 400

    #catalog_item_response = requests.get(f'http://localhost:5222/api/v1/Catalog/items/{data["CatalogItemId"]}')
    catalog_item_response = requests.get(f'http://catalog-api/api/v1/Catalog/items/{data["CatalogItemId"]}')

    if catalog_item_response.status_code == 200:
        content_string = catalog_item_response.text
        data["catalogItem"] = json.loads(content_string)

        basket_content = json.dumps(data)
        headers = {'Content-Type': 'application/json'}
        basket_response = requests.post('http://localhost:5221/api/v1/Direct/items', data=basket_content, headers=headers)
        #basket_response = requests.post('http://basket-api/api/v1/Direct/items', data=basket_content, headers=headers)

        if basket_response.status_code == 200:
            return '', 200

    return '', 500

@retry(stop=stop_after_attempt(2), wait=wait_fixed(1))
def make_request(method, url, data, headers):
    return requests.request(method=method, url=url, data=data, headers=headers)

@app.route('/', defaults={'path': ''}, methods=['GET', 'POST', 'PUT', 'DELETE'])
@app.route('/<path:path>', methods=['GET', 'POST', 'PUT', 'DELETE'])
def reverse_proxy(path):
    if path[:2] == "c/":
        path = path.replace("c/", "/")
        #target_url = 'http://localhost:5222' + path
        #replica_url = 'http://localhost:5232' + path

        target_url = 'http://localhost:7101' + path
        replica_url = 'http://localhost:7121' + path
        
        #target_url = 'http://catalog-api' + path
    elif path[:2] == "b/":
        path = path.replace("b/", "/")
        #target_url = 'http://localhost:5221' + path
        #replica_url = 'http://localhost:5231' + path

        target_url = 'http://localhost:7103' + path
        replica_url = 'http://localhost:7123' + path

        #target_url = 'http://basket-api' + path

    try:
        response = make_request(request.method, target_url, data=request.get_data(), headers=dict(request.headers))
    except Exception:
        # Retry failed; fallback to the replica
        response = requests.request(request.method, replica_url, data=request.get_data(), headers=dict(request.headers))

    return app.response_class(
        response=response.content,
        status=response.status_code,
    )

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=4449)