# مرحلة البناء باستخدام SDK الخاص بـ .NET 10
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# نسخ ملف المشروع واسترجاع الحزم
COPY ["PatientService.csproj", "./"]
RUN dotnet restore "PatientService.csproj"

# نسخ باقي الملفات وبناء التطبيق
COPY . .
RUN dotnet publish "PatientService.csproj" -c Release -o /app/publish

# مرحلة التشغيل النهائي (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PatientService.dll"]