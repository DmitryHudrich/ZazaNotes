FROM python:3.12

WORKDIR /ZazaBot
COPY . .

RUN pip3 install --upgrade setuptools \
    && pip3 install --upgrade pip
    && pip3 install -r requirements.txt
    && pip3 install emoji
    && pip3 install cachetools
    && python -m venv venv
    && source ./venv/bin/activate

ENTRYPOINT ["python", "main.py"]
