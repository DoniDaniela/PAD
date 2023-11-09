FROM python:3.9-slim

WORKDIR python-docker

COPY "ApiGateways/WebAggregator" .
RUN pip3 install -r requirements.txt

CMD [ "python3", "-m" , "flask", "run", "--host=0.0.0.0"]