FROM mcr.microsoft.com/dotnet/sdk:5.0
# Update repositories to include the contrib & nonfree ones (Microsoft fonts have a license)
RUN sed -i "s/main/main contrib nonfree/g" /etc/apt/sources.list && apt-get update
# Install Microsoft fonts and GDIPlus, to try and get around this: https://github.com/aspose-pdf/Aspose.PDF-for-.NET-Old/issues/22
RUN apt-get install -y ttf-mscorefonts-installer && apt-get install -y libgdiplus
