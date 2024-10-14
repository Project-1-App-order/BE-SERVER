# Sử dụng image chính thức của ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Sử dụng image SDK để xây dựng ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["api.csproj", "./"]
RUN dotnet restore "api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "api.csproj" -c Release -o /app/build

# Giai đoạn publish
FROM build AS publish
RUN dotnet publish "api.csproj" -c Release -o /app/publish

# Giai đoạn chạy ứng dụng
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Đặt biến môi trường cho thư mục lưu trữ Data Protection Keys
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DataProtectionKeysPath=/root/.aspnet/DataProtection-Keys

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "api.dll"]
