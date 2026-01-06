This is our Chirp! application for the Authumn 2025 "Analysis, Design and Software Architecture" (BDSA) course at ITU.

## To run chirp locally:

Make sure you have the following downloaded: <br>
[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)


To run _Chirp!_: <br> 
1\. Clone the repository to your directory of choice
    
```
git clone https://github.com/ITU-BDSA2025-GROUP16/Chirp.git
``` 
    
2\. Set up user-secrets _(required for Github OAuth login)_ <br>
2.1. Access your [github developer settings](https://github.com/settings/developers) <br>
2.2. Click **New OAuth App** <br>
2.3. Fill in the following: 

**Application name:** Chirp Local <br>
**Homepage URL:** http://localhost:5696 <br>
**Authorization callback URL:** http://localhost:5696/signin-github <br>

2.4. Click **Update application** <br>
2.5. Copy your _Client ID_, and generate a _Client secret_ <br>
2.6. Finally, set your secrets:

    cd src/Chirp.Web
    dotnet user-secrets set "authentication:github:clientId" "<your-client-id>"
    dotnet user-secrets set "authentication:github:clientSecret" "<your-client-secret>"

3\. While still in the chirp.web directory, run the project
```
dotnet run
``` 

4\. The web application can be accessed on http://localhost:5696 in a browser. 
You should see the _Chirp!_ public page here. 




## Developers:
"Aksel Emil Dyhr <akdy@itu.dk>" <br>

"Samuel Madsen <slma@itu.dk>" <br>
 
"Victor Svendsen <Visv@itu.dk>" <br>

"Torkil á Torkilsheyggi <toat@itu.dk>" <br>

"Sune hesselberg <shes@itu.dk>" 


## Known Issues:
- GitHub Actions currently fails when running multiple test suites concurrently. Tests pass when run individually locally.
