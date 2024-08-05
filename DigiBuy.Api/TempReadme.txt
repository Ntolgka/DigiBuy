dotnet ef migrations list --startup-project ".\DigiBuy.Api\"
dotnet ef database update 20240805200032_InitialCreate --startup-project ".\DigiBuy.Api\"

dotnet ef migrations add InitialCreate -s ..\DigiBuy.Api 

