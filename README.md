This is our Chirp! application for the Authumn 2025 "Analysis, Design and Software Architecture" (BDSA) course at ITU

To run Chirp! locally:
1. Clone to your directory of choice: 
git clone https://github.com/ITU-BDSA2025-GROUP16/Chirp.git

2. If required, set up github user-secrets
(Jump to github user-secrets chapter below)

4. Lastly, proceed to the src/chirp.web directory and execute:
dotnet run


Github user secrets: 
2.1. Access your github developer settings
2.2. Click "New OAuth App"
2.3. Fill in the following:
Application name: Chirp Local
Homepage URL: http://localhost:5696
Authorization callback URL: http://localhost:5696/signin-github

2.4. Click "Update application"
2.5. Copy your Client ID, and generate a Client secret
2.6. Finally, set your secrets:
cd src/Chirp.Web
dotnet user-secrets set "authentication:github:clientId" "<your-client-id>"
dotnet user-secrets set "authentication:github:clientSecret" "<your-client-secret>"


Developers:
"Aksel Emil Dyhr <akdy@itu.dk>"
"Samuel Madsen <slma@itu.dk>"
"Victor Svendsen <Visv@itu.dk>"
"Torkil á Torkilsheyggi <toat@itu.dk>"
"Sune hesselberg <shes@itu.dk>"
