---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2025 Group `<no>`
author:
- "Helge Pfeiffer <ropf@itu.dk>"
- "Adrian Hoff <adho@itu.dk>"
numbersections: true
---

# Design and Architecture of _Chirp!_

## Domain model

![domain-model](https://hackmd.io/_uploads/BkHKww37Wx.png)


## Architecture — In the small - Sam 

![Onion Architecture ](https://hackmd.io/_uploads/HkCBVVxEZe.png)
Each layer of the onion architecture is represented by a .NET project or a .NET test project. Within each project, there may be additional folders or subfolders that can only reference or implicitly use other projects. Each layer in the architecture depends on the inner projects.

## Architecture of deployed application - Victor 

![TP4_Ry8m4CLtI_uEHxg0AihUeIAOz0zAASIHkdHnuYkr59p8JahLgj-zjWc4fHuiw_oxzzxPSziAqtRxYX9HLVLXhxwKtb52oKZYkz88MSBoUFsG9b1Mmbf_fMk94B5AeMvXYP-gn3SzViK25gO-M3IJ28HrV01-A06t1fs8VIZJZC0tn4tZMZCKPGlNiE9kUyoVqndZ_5B4OPJy3jxnTs](https://hackmd.io/_uploads/S1LS5wgN-x.svg)

The Chirp! web app runs in a browser as the client, sending HTTPS requests to the .NET server deployed on Azure App Service. The server handles business logic, processes requests, and interacts with the Azure SQL Database to store and retrieve persistent data such as users, posts, and follows.



## User activities - Aksel


The first diagram below illustrates the options available to a non-authenticated user, starting from the public page.

The second diagram illustrates the options available to users who have been authenticated.

For clarity, Activities have been colored either blue or green. Blue represents states/displays of the application. Green represents buttons/actions available to the user, such as _"click: public page"_ <br>
The following and like features are available across multiple pages in the application. However, due to the fact that the flow of those features is the same regardless of where they are performed, they are only represented once in the diagram.

<center>

![NonAuthenticatedUser](https://hackmd.io/_uploads/r1gIN8gE-g.png)

_Figure 3: Non-authenticated user activities_

</center>
    
<center>
    
![AuthenticatedUser](https://hackmd.io/_uploads/HkpUEUe4-g.png)

_Figure 4: Authenticated user activities_

</center>
    
## Sequence of functionality/calls trough _Chirp!_ - Torkil

The following sequence diagram illustrates the request flow when a client accesses the public page of the application. It shows how an HTTP request is processed through the web layer, page model, application services, infrastructure, and database, and how data is returned and rendered as an HTML response.

![SequenceDiagram](https://hackmd.io/_uploads/HkhPzVxVbg.png)




# Process

## Build, test, release, and deployment - Victor

### Build & Test Diagram 

![Swimlane buildandtest](https://hackmd.io/_uploads/Hy1OEvlNZl.jpg)

This activity diagram illustrates the continuous integration workflow for the Chirp! application. When a developer pushes changes or opens a pull request on the main branch, GitHub triggers a CI workflow. The workflow checks out the repository, sets up the .NET environment, restores dependencies, builds the solution, and runs both core and API tests. The process includes a decision point where the workflow either fails if any test fails or completes successfully if all tests pass.


### Build & Deploy to Azure

![azure diagram](https://hackmd.io/_uploads/B1XKEvlEbg.jpg)

This activity diagram shows the continuous deployment workflow for Chirp!. When a developer pushes changes to the main branch, GitHub triggers a deployment workflow. The application is built and published using GitHub Actions, and the resulting artifact is uploaded and passed to a deployment job. The workflow then authenticates with Azure and deploys the application to the production slot of an Azure Web App. Once deployed, the updated application is running in the production environment.


### Release Workflow
![releasediagram](https://hackmd.io/_uploads/SJ2YVwxE-x.jpg)

This activity diagram describes the release workflow for Chirp!, which is triggered when a version tag is pushed to the repository. The workflow builds and publishes the application for multiple operating systems in parallel, including Linux, Windows, and macOS. Each build is packaged as a ZIP artifact and uploaded. After all builds complete, a GitHub Release is created, the artifacts are attached, and release notes are generated automatically. The final result is a published versioned release available for download.

## Team work - Torkil

Show a screenshot of your project board right before hand-in. Briefly describe which tasks are still unresolved, i.e., which features are missing from your applications or which functionality is incomplete.

Briefly describe and illustrate the flow of activities that happen from the new creation of an issue (task description), over development, etc. until a feature is finally merged into the main branch of your repository.


An issue would be created from the requirements given in the course. It would then be added to our Git to-do list. From there we would assign issues to different group members. Mostly just one person per issue. When a task is completed a pull request is made, and from there another group member reviews the work, and decides if it is acceptable. If it is, the branch is merged into the main branch, and if it is not acceptable, the reviewer tells the group member what is wrong and what has to be changed. From there the group member has to make a pull request again, when the task is complete.


![Screenshot 2025-12-29 182901](https://hackmd.io/_uploads/Skcjq4gEZe.png)



## How to make _Chirp!_ work locally - Aksel

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



## How to run test suite locally - Aksel

The project has 3 seperate test suites: 
* ApiTests 
* Chirp.Web.Tests 
* CoreTests

From the root, first
```
dotnet restore
dotnet build
```
Then access your desuired test suite, e.g.
```
cd tests/ApiTests
```
Then run the tests inside it
```
dotnet test
```
The suites contain unit tests, integration tests, and E2E tests.  <br>
With these, the system's core components (pages, repositories, services, etc.) are tested both individually and when working together. <br>
The E2E tests target two specific user flows: liking cheeps and following users. 


# Ethics - sam

## License

Chirp is licensed under MIT.

## LLMs, ChatGPT, CoPilot, and others

As large language models (LLMs) have become increasingly integrated into the creative process, we have naturally turned to them for inspiration and guidance regarding assignment requirements. Our group has prioritized transparency by consistently co-authoring with LLMs whenever they are utilized. Generally, we have used LLMs not for coding purposes but as sources of inspiration and as a collaborative partner. 

