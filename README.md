# Action Language Swap

Plugin Dalamud pour Final Fantasy XIV qui affiche les noms des actions (et, en option, des statuts)
dans une autre langue que celle du client. Exemple : jeu en français, noms des actions en anglais.

Remplacements : barres d'incantation (toi, cible, cible focus, groupe, ennemis), infobulle et menu des actions,
textes volants, journal de combat. Configuration : `/actionlang`.

## Installation

1. En jeu, tape `/xlsettings` puis ouvre l'onglet **Experimental**.
2. Dans **Custom Plugin Repositories**, colle l'adresse suivante, clique sur **+**, coche **Enabled** puis enregistre :

   ```
   https://raw.githubusercontent.com/Soratix/ActionLangSwap/main/repo.json
   ```

3. Tape `/xlplugins`, cherche **Action Language Swap** et installe-le.

## Publier une nouvelle version

```powershell
.\publish.ps1 -Version 0.2.0 -Notes "Ce qui a changé"
```

Le script compile, met à jour `repo.json`, crée le commit et le tag, pousse sur GitHub et
crée la release avec `latest.zip`.
