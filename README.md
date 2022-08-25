# SubtitleDownloadCore

Subtitle Downloader / C# / .Net 6 / xUnit

---

## Usage

> ``` dotnet SubtitleDownloadCore.dll [path-to-movies-directory] ```


Then it will try to download the subtitles (pt, en) for all the movie files it finds in the subdirectories.


### OpenSubtitles API

To use the OpenSubtitles API, you will need a API-Key

To setup one : 

1. Create an account : https://www.opensubtitles.com/en/users/sign_in

2. Generate a API-Key : https://www.opensubtitles.com/en/consumers

3. Setup the API-Key :

    3.1 Standalone use 

    > Set the API-Key in the appsettings.json and run the project 

    3.2 Development use

    > Setup the API-Key using dotnet user-secrets  (recommended)

    ```
    dotnet user-secrets init
    dotnet user-secrets set "OpenSubtitlesApiKey" "[your API-Key here]"
    ```
