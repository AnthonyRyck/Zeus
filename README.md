# Zeus
C'est une application pour suivre et partager votre bibliothèque de films en .Net Blazor.

<p align="center"><kbd><img src="https://image.ibb.co/jWZYfK/Zeus_0.jpg" height="450"></kbd></p>

#### Disponible sur le Playstore
Il y a une application sur le [Playstore](https://play.google.com/store/apps/details?id=com.anthonyryck.zeus). Il faut renseigner l'adresse d'un serveur Zeus, et il est possible de consulter la liste des films.

<p align="center">
    <kbd>
        <img src="https://play-lh.googleusercontent.com/NpA33zEJMFPIth9v-2Xh9V6m7HIGofOW1oxN-MFim6F5F1cx2VAzb64d23Evh3_fIzU=w720-h310-rw">
    </kbd>
</p>

Vous indiquez à l'application ou trouver les films, les séries, et elle se charge de retrouver toutes les informations avec l'API de [**The Movie Database**](https://www.themoviedb.org/?language=fr).

## Fonctionnalitées

#### Partage

Il est possible de créer des comptes utilisateurs avec un rôle de membre ou de manager.

##### Membre
Le rôle membre peut télécharger une vidéo.

##### Manager
Le rôle manager est comme un membre, et il peut en plus corriger les mauvaises associations de film.

## Installation
C'est une application en .Net Core, du coup vous pouvez passer soit par :
- le [**docker hub**](https://hub.docker.com/r/anthonyryck/zeus/) pour utiliser l'image.
- l'exécution de l'application en ligne de commande :
```cmd
    C:\Path to Code Source\...\WebAppServer>dotnet run    
```
Cette commande compile et exécute le l'application.

Docker compose
```yml
version: "3.7"
services:
  zeuscompose:
     image: anthonyryck/zeus:latest
     container_name: zeuscompose
     hostname: zeuscompose
     expose :
       - 80
     volumes:
       - /pathToYourMovies/Movies:/app/movies
       - /pathToSaveDatabaseSqlite/database:/app/Database
       - /pathToSaveConfigFile/conf:/app/config
       - /pathToSaveMoviesConf/save:/app/save
       - /pathToSaveLogFiles/log:/app/Logs
```
