FROM microsoft/mssql-server-linux

WORKDIR sqlserver-workdir

COPY . .

ENV ACCEPT_EULA Y
ENV SA_PASSWORD AdminPassword1

RUN ln -s /opt/mssql-tools/bin/sqlcmd /usr/bin 
RUN chmod +x /sqlserver-workdir/init-db.sh

CMD /bin/bash ./entrypoint.sh 
