# Configuration MCP pour GitHub Copilot

Ce projet inclut un serveur MCP (Model Context Protocol) qui expose des outils Pomodoro et de génération de nombres aléatoires à GitHub Copilot.

## 🚀 Configuration automatique

Le fichier `.vscode/mcp.json` est déjà configuré pour ce projet. GitHub Copilot devrait automatiquement détecter et utiliser le serveur MCP.

## 🛠️ Outils disponibles

Une fois le serveur MCP activé, vous pouvez utiliser ces commandes avec GitHub Copilot :

### 🍅 Outils Pomodoro

1. **Session Console** : `"Lance une session Pomodoro de 25 minutes"`
2. **Interface Graphique** : `"Démarre un Pomodoro avec interface graphique"`
3. **Arrêt** : `"Arrête ma session Pomodoro"`
4. **Statut** : `"Quel est le statut de mon Pomodoro ?"`
5. **Langues** : `"Quelles langues sont disponibles pour le Pomodoro ?"`

### 🎲 Outils utilitaires

- **Nombres aléatoires** : `"Donne-moi 3 nombres aléatoires"`

## 🎯 Exemples d'utilisation

Dans le chat GitHub Copilot, vous pouvez maintenant taper :

```
- "Lance un Pomodoro de 30 minutes avec interface graphique"
- "Démarre une session de travail de 45 minutes en français"
- "Ouvre l'interface Pomodoro pour une session de 25 minutes"
- "Arrête mon timer Pomodoro actuel"
- "Génère 5 nombres aléatoires entre 1 et 100"
```

## 🔧 Configuration manuelle (si nécessaire)

Si le serveur MCP n'est pas automatiquement détecté :

1. **Redémarrez VS Code** après avoir ouvert ce projet
2. **Vérifiez la configuration** dans `.vscode/mcp.json`
3. **Activez MCP** dans les paramètres GitHub Copilot

## 📋 Structure de configuration

```json
{
  "servers": {
    "POC-MCP-Server": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "src/SampleMcpServer"
      ]
    }
  }
}
```

## 🐛 Dépannage

- **Le serveur ne démarre pas** : Vérifiez que .NET 9.0 est installé
- **Les outils ne sont pas disponibles** : Redémarrez VS Code et ouvrez le chat Copilot
- **Interface graphique ne se lance pas** : Utilisez la version console en attendant

## ✅ Vérification du fonctionnement

1. Ouvrez le chat GitHub Copilot dans VS Code
2. Tapez : `"Lance un Pomodoro avec interface graphique"`
3. Copilot devrait proposer d'utiliser l'outil `start_pomodoro_graphical`
4. Acceptez et l'interface graphique du Pomodoro devrait s'ouvrir !

---

🎊 **Le serveur MCP est maintenant intégré à GitHub Copilot pour ce projet !**