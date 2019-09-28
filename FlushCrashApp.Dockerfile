FROM r-base as base
RUN apt-get update; apt-get --assume-yes install gnupg ; \
    wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.asc.gpg; \
    mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/ ; \
    wget -q https://packages.microsoft.com/config/debian/9/prod.list ; \
    mv prod.list /etc/apt/sources.list.d/microsoft-prod.list  ; \
    apt-get update

RUN apt-get --assume-yes install dotnet-sdk-2.1
WORKDIR /
COPY . .
RUN dotnet publish TestApps/FlushCrashApp/FlushCrashApp.csproj -c Release -o out
WORKDIR TestApps/FlushCrashApp/out/
ENV "R_HOME" "/usr/lib/R"
RUN [ "dotnet", "FlushCrashApp.dll" ]