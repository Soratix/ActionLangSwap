# Action Language Swap

Dalamud plugin for Final Fantasy XIV that shows action names in another language than your game client.
Example: French client, English action names.

*Version française plus bas.*

## Features

- Choose the language of action names: English, French, German or Japanese.
- Optionally translate status names too (buffs / debuffs).
- Names are replaced in:
  - cast bars (yours, target, focus target, party list, enemy list);
  - action tooltips and the actions menu;
  - flying text (damage / healing);
  - the battle log.
- Each location can be turned on or off.
- The plugin interface follows Dalamud's language (English, French, German, Japanese; English otherwise).

## Installation

1. In game, type `/xlsettings` and open the **Experimental** tab.
2. Under **Custom Plugin Repositories**, add the following URL, tick **Enabled** and save:

   ```
   https://soratix.fr/ffxiv/repo.json
   ```

3. Type `/xlplugins`, search for **Action Language Swap** and install it.

## Usage

Type `/actionlang` (or use the settings button in `/xlplugins`) to choose the language and where names are replaced.

## Limitations

- Action names quoted inside tooltip descriptions are not translated.
- In the battle log, very common names (for example a basic "Attack") may also be replaced inside sentences.

---

## Français

Plugin Dalamud pour Final Fantasy XIV qui affiche les noms des actions dans une autre langue que celle du jeu.
Exemple : jeu en français, noms des actions en anglais.

### Fonctionnalités

- Langue des noms d'actions au choix : anglais, français, allemand ou japonais.
- Option pour traduire aussi les statuts (buffs / debuffs).
- Remplacement dans les barres d'incantation (toi, cible, cible focus, groupe, ennemis), l'infobulle
  et le menu des actions, les textes volants (dégâts / soins) et le journal de combat. Chaque endroit
  peut être activé ou désactivé.
- L'interface du plugin suit la langue de Dalamud (anglais par défaut).

### Installation

1. En jeu, tape `/xlsettings` puis ouvre l'onglet **Experimental**.
2. Dans **Custom Plugin Repositories**, ajoute l'adresse `https://soratix.fr/ffxiv/repo.json`,
   coche **Enabled** puis enregistre.
3. Tape `/xlplugins`, cherche **Action Language Swap** et installe-le.

### Utilisation

Tape `/actionlang` pour choisir la langue et les endroits où les noms sont remplacés.

### Limites

- Les noms d'actions cités dans le texte des descriptions d'infobulles ne sont pas traduits.
- Dans le journal de combat, un nom très courant (comme « Attaque ») peut aussi être remplacé dans une phrase.
