# Zeus
C'est une application pour suivre et partager votre bibliothèque de films en .Net Blazor.

<p align="center"><kbd><img src="https://image.ibb.co/jWZYfK/Zeus_0.jpg" height="450"></kbd></p>

Vous indiquez à l'application ou trouver les films, les séries, et elle se charge de retrouver toutes les informations avec l'API de [**The Movie Database**](https://www.themoviedb.org/?language=fr).

## Fonctionnalitées

#### Partage

Il est possible de créer des comptes utilisateurs avec un rôle de membre ou de manager.

##### Membre
Le rôle membre peut télécharger une vidéo.

##### Manager
Le rôle manager est comme un membre, et il peut en plus corriger les mauvaises associations de film.

#### Pas encore implémenté
- La "WhishList". Les utilisateurs pourront ajouter dans cette liste, les films qu'ils désirent avoir.
- Les utilisateurs recevront un mail quand un nouveau film est ajouté.

## Installation
C'est une application en .Net Core, du coup vous pouvez passer soit par :
- le [**docker hub**](https://hub.docker.com/r/anthonyryck/zeus/) pour utiliser l'image que j'ai créé.
- l'exécution de l'application en ligne de commande :
```
    C:\Path to Code Source\...\WebAppServer>dotnet run    
```
Cette commande compile et exécute le l'application.
