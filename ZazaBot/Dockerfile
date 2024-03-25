FROM python:3.12

WORKDIR /ZazaBot
COPY requirements.txt requirements.txt

RUN pip3 install --upgrade setuptools
RUN pip3 install --upgrade pip
RUN pip3 install -r requirements.txt
RUN pip3 install emoji
RUN pip3 install cachetools

COPY . .


ENTRYPOINT ["python", "main.py"]