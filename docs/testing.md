# Tests Documentation

Ce document décrit la stratégie de test et l'organisation des tests pour le projet SampleMcpServer.

## Organisation des Tests

Le projet de test `SampleMcpServer.Tests` est organisé comme suit :

```
tests/SampleMcpServer.Tests/
├── Tools/                     # Tests unitaires pour les outils MCP
│   └── RandomNumberToolsTests.cs
├── Integration/               # Tests d'intégration
│   └── ProgramIntegrationTests.cs
├── GlobalUsings.cs           # Using globaux pour xUnit et FluentAssertions
└── SampleMcpServer.Tests.csproj
```

## Framework de Test

- **Framework** : xUnit 2.9.2
- **Assertions** : Assertions natives xUnit (Assert.*)
- **Couverture de Code** : coverlet.collector
- **Mocking** : Moq 4.20.69 (si nécessaire)

## Types de Tests

### Tests Unitaires

Les tests unitaires se trouvent dans le dossier `Tools/` et testent les classes d'outils MCP individuellement.

#### RandomNumberToolsTests

Cette classe teste la classe `RandomNumberTools` avec les scénarios suivants :

- **Plages par défaut** : Teste le comportement avec les paramètres par défaut (0-100)
- **Plages personnalisées** : Teste différentes plages de valeurs
- **Variabilité** : Vérifie que le générateur produit des résultats variés
- **Cas limites** : Teste le comportement avec min == max et min > max
- **Métadonnées MCP** : Vérifie la présence des attributs `[McpServerTool]` et `[Description]`
- **Signatures** : Vérifie les types de retour et paramètres

### Tests d'Intégration

Les tests d'intégration se trouvent dans le dossier `Integration/` et testent l'assemblage complet de l'application.

#### ProgramIntegrationTests

Cette classe teste :

- **Construction du Host** : Vérifie que l'application peut être configurée sans erreur
- **Enregistrement des Services** : Vérifie que les services MCP sont correctement enregistrés
- **Démarrage/Arrêt** : Teste le cycle de vie de l'application
- **Configuration du Logging** : Vérifie la configuration des logs

## Exécution des Tests

### Commandes de Base

```bash
# Exécuter tous les tests
dotnet test

# Exécuter avec la couverture de code
dotnet test --collect:"XPlat Code Coverage"

# Exécuter des tests spécifiques
dotnet test --filter "ClassName=RandomNumberToolsTests"

# Mode verbeux
dotnet test --verbosity normal
```

### Couverture de Code

La couverture de code est collectée automatiquement via `coverlet.collector`. Les rapports sont générés au format Cobertura dans le dossier `TestResults/`.

## Bonnes Pratiques

### Nomenclature

- **Classes de test** : `{ClasseTestée}Tests`
- **Méthodes de test** : `{Méthode}_{Scénario}_{RésultatAttendu}`
- **Exemples** :
  - `GetRandomNumber_WithDefaultParameters_ReturnsNumberInExpectedRange`
  - `GetRandomNumber_WithInvalidRange_ThrowsArgumentOutOfRangeException`

### Structure des Tests

Utiliser le pattern **Arrange-Act-Assert** :

```csharp
[Fact]
public void GetRandomNumber_WithDefaultParameters_ReturnsNumberInExpectedRange()
{
    // Arrange
    var tools = new RandomNumberTools();

    // Act
    var result = tools.GetRandomNumber();

    // Assert
    Assert.True(result >= 0);
    Assert.True(result < 100);
}
```

### Tests Paramétrés

Utiliser `[Theory]` et `[InlineData]` pour tester plusieurs scénarios :

```csharp
[Theory]
[InlineData(1, 10)]
[InlineData(50, 100)]
[InlineData(0, 1)]
public void GetRandomNumber_WithCustomRange_ReturnsNumberInRange(int min, int max)
{
    // Test implementation
}
```

## Intégration CI/CD

Les tests sont automatiquement exécutés dans les workflows GitHub Actions :

- **Build workflow** : Exécute `dotnet test` sur tous les environnements
- **Couverture** : Les rapports de couverture peuvent être intégrés avec des outils comme Codecov
- **Échec de build** : Si un test échoue, le build échoue

## Ajout de Nouveaux Tests

### Pour une nouvelle classe d'outils MCP

1. Créer `tests/SampleMcpServer.Tests/Tools/{NomClasse}Tests.cs`
2. Tester tous les outils MCP de la classe
3. Vérifier les attributs MCP (`[McpServerTool]`, `[Description]`)
4. Tester les cas limites et les exceptions

### Pour les tests d'intégration

1. Ajouter les tests dans `Integration/`
2. Utiliser le vrai container DI de l'application
3. Tester les interactions entre composants
4. Éviter les dépendances externes (bases de données, services web)

## Configuration

### Global Usings

Le fichier `GlobalUsings.cs` définit les using globaux :

```csharp
global using FluentAssertions;
global using Xunit;
```

### InternalsVisibleTo

Le projet principal expose ses types internes aux tests via :

```xml
<ItemGroup>
  <InternalsVisibleTo Include="SampleMcpServer.Tests" />
</ItemGroup>
```

## Débogage des Tests

### Dans VS Code

1. Ouvrir le fichier de test
2. Placer des points d'arrêt
3. Utiliser "Debug Test" dans la fenêtre Test Explorer

### En ligne de commande

```bash
# Exécuter un test spécifique avec débogage
dotnet test --filter "TestName" --logger "console;verbosity=detailed"
```

## Métriques de Qualité

- **Couverture de code** : Objectif > 80%
- **Tests par fonctionnalité** : Minimum 3 tests (nominal, limite, erreur)
- **Temps d'exécution** : Tests unitaires < 100ms, intégration < 1s
- **Aucun test ignoré** : Tous les tests doivent passer ou être corrigés