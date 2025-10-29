# Pomodoro Timer

Une application console et graphique qui implémente la technique Pomodoro avec des sessions de travail et de pause personnalisables. **Disponible en 6 langues et 2 modes d'interface !**

## Utilisation

```bash
dotnet run --project src/PomodoroTimer -- [options] <temps_travail_minutes> <temps_pause_minutes>
```

## Options

- `--ui` : Lance l'interface graphique au lieu de la console
- `--lang=LANGUE` : Définit la langue de l'interface (en, fr, es, zh, ja, ru)

### Exemples

```bash
# Session Pomodoro classique en mode console (français) : 25 minutes de travail, 5 minutes de pause
dotnet run --project src/PomodoroTimer -- --lang=fr 25 5

# Session avec interface graphique en anglais : 15 minutes de travail, 3 minutes de pause
dotnet run --project src/PomodoroTimer -- --lang=en 15 3 --ui

# Session avec interface graphique : 30 minutes de travail, 10 minutes de pause
dotnet run --project src/PomodoroTimer -- 30 10 --ui

# Session console sans langue spécifiée (anglais par défaut) : 45 minutes de travail, 10 minutes de pause
dotnet run --project src/PomodoroTimer -- 45 10
```

## 🌍 Support des langues

- **�🇧 Anglais** : `--lang=en` **(par défaut)**
- **�🇫🇷 Français** : `--lang=fr`
- **�� Espagnol** : `--lang=es`
- **�🇳 Chinois** : `--lang=zh`
- **🇯🇵 Japonais** : `--lang=ja`
- **🇷🇺 Russe** : `--lang=ru`

## Fonctionnalités

### 🖥️ Deux modes d'interface

**Mode Console (par défaut)**
- ⏱️ **Compte à rebours en temps réel** : Affichage du temps restant toutes les secondes
- � **Interface claire** : Affichage des phases (TRAVAIL/PAUSE) avec émojis
- 🔊 **Signal sonore** : Bip à la fin de chaque phase (mode standalone uniquement)

**Mode Interface Graphique (`--ui`)**
- 🪟 **Fenêtre dédiée** : Interface graphique moderne avec Avalonia
- 📊 **Barre de progression** : Visualisation du temps restant
- 🎨 **Changements de couleur** : Interface qui change selon la phase (vert=travail, orange=pause)
- ⏸️ **Contrôles interactifs** : Boutons Pause/Resume et Stop
- � **Notifications** : Boîtes de dialogue à la fin de chaque phase
- � **Toujours au premier plan** : La fenêtre reste visible

### 🌍 Autres fonctionnalités

- 🔄 **Sessions illimitées** : L'application continue jusqu'à ce que vous l'arrêtiez
- ⚡ **Paramétrable** : Définissez vos propres durées de travail et de pause
- 🌍 **Multilingue** : Interface disponible en 6 langues avec anglais par défaut
- 🔄 **Fallback automatique** : Si l'interface graphique ne peut pas démarrer, passage automatique en mode console

## Commandes

### Mode Console
- **Arrêter** : `Ctrl+C` pour quitter l'application
- **Continuer** : Appuyer sur n'importe quelle touche pour passer à la session suivante après une pause

### Mode Interface Graphique
- **Pause/Resume** : Clic sur le bouton "Pause" pour mettre en pause ou reprendre
- **Arrêter** : Clic sur le bouton "Stop" ou fermer la fenêtre pour quitter
- **Continuer** : Clic sur "Yes" dans la boîte de dialogue à la fin de chaque phase

### Options communes
- **Langue** : `--lang=CODE` où CODE peut être : `en`, `fr`, `es`, `zh`, `ja`, `ru`
- **Interface** : `--ui` pour lancer l'interface graphique

## Technique Pomodoro

La technique Pomodoro est une méthode de gestion du temps qui utilise un minuteur pour diviser le travail en intervalles, traditionnellement de 25 minutes, séparés par de courtes pauses.

### Configuration classique
- 🍅 25 minutes de travail
- ☕ 5 minutes de pause
- Après 4 sessions, pause longue de 15-30 minutes

## Exemples de messages dans différentes langues

### En anglais (par défaut)
```
🍅 Pomodoro Session - Work: 25min, Break: 5min
Press Ctrl+C to stop

🔥 SESSION 1 - WORK (25 minutes)
WORK: 24:59 remaining
```

### En français
```
🍅 Session Pomodoro - Travail: 25min, Pause: 5min
Appuyez sur Ctrl+C pour arrêter

🔥 SESSION 1 - TRAVAIL (25 minutes)
TRAVAIL: 24:59 restant
```

### En chinois
```
🍅 番茄钟会话 - 工作: 25分钟, 休息: 5分钟
按 Ctrl+C 停止

🔥 第 1 个会话 - 工作 (25 分钟)
工作: 24:59 剩余
```

### En japonais
```
🍅 ポモドーロセッション - 作業: 25分, 休憩: 5分
Ctrl+C で停止

🔥 セッション 1 - 作業 (25 分)
作業: 24:59 残り
```