FROM microsoft/mssql-tools

RUN apt-get install redis-tools -y

WORKDIR /shell-scripts

COPY . .

CMD sh ./init-db.sh