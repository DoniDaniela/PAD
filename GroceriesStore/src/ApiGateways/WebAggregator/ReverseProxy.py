import http.server
import http.client
import socketserver

class ReverseProxy(http.server.BaseHTTPRequestHandler):
    def do_GET(self):
        self.proxy_request()

    def do_POST(self):
        self.proxy_request()

    def proxy_request(self):
        # Define the target server and port you want to proxy requests to
        target_host = 'localhost'
        target_port = 5222

        if self.path[:3] == "/c/":
            self.path = self.path.replace("/c/", "/")
        elif self.path[:3] == "/b/":
            self.path = self.path.replace("/b/", "/")
            target_port = 5221

        # Create an HTTP connection to the target server
        connection = http.client.HTTPConnection(target_host, target_port)

        # Build the request headers
        request_headers = {header: value for header, value in self.headers.items()}

        if self.command == 'POST':
            # Read the request body and send it to the target server
            content_length = int(request_headers.get('Content-Length', 0))
            request_body = self.rfile.read(content_length)
            connection.request('POST', self.path, body=request_body, headers=request_headers)
        else:
            # For GET requests, just forward the request to the target server
            connection.request('GET', self.path, headers=request_headers)

        response = connection.getresponse()

        # Send the response back to the client
        self.send_response(response.status)
        #for header, value in response.getheaders():
        #    self.send_header(header, value)
        self.end_headers()
        self.wfile.write(response.read())

if __name__ == '__main__':
    PORT = 4449

    with socketserver.TCPServer(("", PORT), ReverseProxy) as httpd:
        print(f"Reverse proxy is running on port {PORT}")
        httpd.serve_forever()
