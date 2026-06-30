# TourneeFutee

Projet PSI A2 S4 consacre a la modelisation de graphes, au calcul de tournees optimales et a la persistance des resultats en base MySQL.

Le coeur du projet est une application .NET 8 en C#. Elle fournit des classes pour manipuler des graphes ponderes, representer des matrices d'adjacence, resoudre un probleme de voyageur de commerce avec l'algorithme de Little et sauvegarder/recharger graphes et tournees.

## Membres du groupe

- Alexandre LESUR
- Loris LIGONNIERE
- Antoine MASCIOTRA

## Fonctionnalites

- Creation de graphes orientes ou non orientes.
- Ajout, suppression et modification de sommets et d'arcs ponderes.
- Representation interne par matrice d'adjacence.
- Manipulation de matrices : insertion/suppression de lignes et colonnes, lecture/ecriture de coefficients, reduction de matrice.
- Calcul d'une tournee optimale avec l'algorithme de Little.
- Representation d'une tournee sous forme de segments et de cout total.
- Sauvegarde et chargement de graphes/tournees dans une base MySQL.
- Tests unitaires MSTest sur les graphes, matrices et l'algorithme de Little.
- Tests d'integration pour la persistance MySQL.

## Structure du depot

```text
.
|-- TourneeFutee.sln
|-- TourneeFutee/
|   |-- Graph.cs                 # Graphe oriente/non oriente et matrice d'adjacence
|   |-- Matrix.cs                # Matrice de couts et operations de base
|   |-- Little.cs                # Algorithme de Little
|   |-- LittleNoeud.cs           # Etat interne d'une branche de Little
|   |-- Tour.cs                  # Resultat d'une tournee
|   |-- Vertex.cs                # Sommet interne du graphe
|   |-- ServicePersistance.cs    # Acces MySQL
|   `-- Program.cs               # Point d'entree console
|-- TourneeFutee.Tests/
|   |-- GraphTests.cs
|   |-- MatrixTests.cs
|   |-- LittleTests.cs
|   `-- PersistanceTestsMAJ2.cs
|-- init_db.sql                  # Initialisation de la base MySQL de test
|-- MetroParis.csv               # Donnees de stations du metro parisien
`-- .gitignore
```

## Diagramme de classes :

```mermaid
classDiagram
    direction LR

    %% --- Point d'entrée ---
    class Program {
        + Main(string[] args)$ void
    }

    %% --- Persistance MySQL ---
    class ServicePersistance {
        - string _connectionString
        - MySqlConnection _connection
        + ServicePersistance(string serverIp, string dbname, string user, string pwd)
        + SaveGraph(Graph g) uint
        + LoadGraph(uint id) Graph
        + SaveTour(uint graphId, Tour t) uint
        + LoadTour(uint id) Tour
        - OpenConnection() MySqlConnection
    }

    %% --- Logique de l'Algorithme ---
    class Little {
        - Graph graph
        + Graph Graph
        + Little(Graph graph)
        + ComputeOptimalTour() Tour
        + ReduceMatrix(Matrix m)$ float
        + GetMaxRegret(Matrix m)$ Tuple
        + IsForbiddenSegment(Tuple segment, List includedSegments, int nbCities)$ bool
    }

    class LittleNoeud {
        - Matrix mat
        - List~Tuple~ edges
        - float cost
        - List~string~ rowLabels
        - List~string~ colLabels
        + Matrix Mat
        + List~Tuple~ Edges
        + float Cost
        + List~string~ RowLabels
        + List~string~ ColLabels
        + LittleNoeud(Matrix mat, List edges, float cost, List rowLabels, List colLabels)
    }

    %% --- Structure de Données ---
    class Graph {
        - int order
        - bool directed
        - float noEdgeValue
        - Dictionary~string, Vertex~ vertices
        - Matrix adjMat
        + int Order
        + bool Directed
        + Matrix AdjMat
        + float NoEdgeValue
        + Graph(bool directed, float noEdgeValue)
        + IsAlreadyVertexExists(string name) bool
        + AddVertex(string name, float value) void
        + RemoveVertex(string name) void
        + GetVertexValue(string name) float
        + SetVertexValue(string name, float value) void
        + GetIntFromVertexName(string name) int
        + GetNeighbors(string vertexName) List~string~
        + AddEdge(string s, string d, float w) void
        + RemoveEdge(string s, string d) void
        + GetEdgeWeight(string s, string d) float
        + SetEdgeWeight(string s, string d, float w) void
        + GetVertexNameFromInt(int id) string
        + ContainsVertex(string name) bool
    }

    class Matrix {
        - int nbRows
        - int nbColumns
        - float defaultValue
        - float[,] mat
        + float DefaultValue
        + int NbRows
        + int NbColumns
        + float[,] Mat
        + float MaxValue
        + Matrix(int nbRows, int nbColumns, float defaultValue)
        + Clone() Matrix
        + AddRow(int i) void
        + AddColumn(int j) void
        + RemoveRow(int i) void
        + RemoveColumn(int j) void
        + GetValue(int i, int j) float
        + SetValue(int i, int j, float v) void
        + Print() void
        + GetMinRow(int i) float
        + GetMinCol(int j) float
        + GetMinRowExcept(int i, int exCol) float
        + GetMinColExcept(int j, int exRow) float
        + OverrideInfinite() Matrix
    }

    class Vertex {
        - string name
        - int index
        - float value
        - List~Vertex~ neighbor
        + string Name
        + int Index
        + float Value
        + List~Vertex~ Neighbor
        + Vertex(string name, int index, float value)
    }

    class Tour {
        - float cost
        - List~Tuple~ segments
        + float Cost
        + int NbSegments
        + List~Tuple~ Segments
        + IList~string~ Vertices
        + Tour(List~Tuple~ segments, float cost)
        + Tour(List~string~ vertices, float cost)
        + ContainsSegment(Tuple segment) bool
        + Print() void
    }

    %% --- Relations ---
    Program ..> ServicePersistance : utilise
    Program ..> Little : utilise

    ServicePersistance ..> Graph : sauvegarde/charge
    ServicePersistance ..> Tour : sauvegarde/charge

    Little "1" o-- "1" Graph : manipule
    Little ..> Tour : produit
    Little ..> LittleNoeud : instancie

    Graph "1" *-- "1" Matrix : possede
    Graph "1" *-- "*" Vertex : contient
    LittleNoeud "1" o-- "1" Matrix : possede copie

    Vertex -- Vertex : voisins
```

## Diagramme Entité/Association de la base de données

```mermaid
classDiagram
    direction TB

    class GRAPHE {
        +int unsigned id [PK]
        +tinyint est_oriente
        +int ordre
        +float noEdgeValue
    }

    class TOURNEE {
        +int unsigned id [PK]
        +int unsigned graphe_id [FK]
        +float cout_total
    }

    class SOMMET {
        +int unsigned id [PK]
        +int unsigned graphe_id [FK]
        +varchar(50) nom
        +float valeur
    }

    class ETAPETOURNEE {
        +int unsigned tournee_id [PK, FK]
        +int unsigned numero_ordre [PK]
        +int unsigned sommet_id [FK]
    }

    class ARC {
        +int unsigned id [PK]
        +int unsigned graphe_id [FK]
        +int unsigned sommet_source [FK]
        +int unsigned sommet_dest [FK]
        +float poids
    }

    %% 1. Squelette principal (Dessiné en premier, traits pleins)
    GRAPHE "1,1" -- "0,N" SOMMET : contient
    SOMMET "1,1" --> "0,N" ARC : source

    GRAPHE "1,1" -- "0,N" TOURNEE : possede
    TOURNEE "1,1" -- "1,N" ETAPETOURNEE : contient

    %% 2. Liens secondaires (Dessinés autour, pointillés)
    SOMMET "1,1" ..> "0,N" ARC : destination
    GRAPHE "1,1" .. "0,N" ARC : contient
    SOMMET "1,1" .. "0,N" ETAPETOURNEE : correspond_a
```

## Prerequis

- .NET SDK 8.0 ou plus recent.
- Visual Studio, Rider, VS Code ou un terminal compatible .NET.
- MySQL uniquement pour les tests et fonctions de persistance.

Les dependances NuGet principales sont :

- `MySql.Data`
- `System.Text.Encoding.CodePages`
- `MSTest`
- `Microsoft.NET.Test.Sdk`

## Installation

Depuis la racine du depot :

```bash
dotnet restore
dotnet build
```

## Lancer le projet

```bash
dotnet run --project TourneeFutee
```

Le point d'entree `Program.cs` est actuellement minimal. Les principales fonctionnalites sont exposees par les classes du projet et validees dans les tests.

## Utilisation rapide

Exemple de creation d'un graphe et de calcul d'une tournee :

```csharp
using TourneeFutee;

Graph graph = new Graph(directed: true);

graph.AddVertex("A");
graph.AddVertex("B");
graph.AddVertex("C");

graph.AddEdge("A", "B", 4);
graph.AddEdge("A", "C", 2);
graph.AddEdge("B", "A", 3);
graph.AddEdge("B", "C", 1);
graph.AddEdge("C", "A", 5);
graph.AddEdge("C", "B", 6);

Little little = new Little(graph);
Tour tour = little.ComputeOptimalTour();

Console.WriteLine($"Cout total : {tour.Cost}");
tour.Print();
```

Pour un calcul Little complet, le graphe doit representer un probleme de voyageur de commerce exploitable : les trajets necessaires doivent etre presents et les couts doivent etre coherents.

## Tests

Executer tous les tests :

```bash
dotnet test
```

Les tests de persistance utilisent une vraie base MySQL locale. Pour lancer uniquement les tests unitaires qui ne dependent pas de MySQL :

```bash
dotnet test --filter "FullyQualifiedName!~PersistanceTests"
```

## Base de donnees MySQL

La classe `ServicePersistance` permet de sauvegarder et charger :

- des graphes dans les tables `Graphe`, `Sommet` et `Arc` ;
- des tournees dans les tables `Tournee` et `EtapeTournee`.

Le script `init_db.sql` initialise une base de test nommee `tourneefutee_test`.

Configuration attendue par les tests d'integration :

```text
Serveur  : 127.0.0.1
Base     : tourneefutee_test
Utilisateur : root
Mot de passe : root
```

Initialisation possible depuis MySQL :

```sql
SOURCE init_db.sql;
```

Ou depuis un terminal, selon votre installation MySQL :

```bash
mysql -u root -p < init_db.sql
```

Adaptez les constantes `DB_SERVER`, `DB_NAME`, `DB_USER` et `DB_PWD` dans `TourneeFutee.Tests/PersistanceTestsMAJ2.cs` si votre environnement local est different.

## Donnees

`MetroParis.csv` contient des donnees de stations du metro parisien :

- identifiant de station ;
- ligne ;
- nom de station ;
- longitude et latitude ;
- commune et code INSEE.

Ce fichier est versionne car il sert de jeu de donnees projet. Il ne doit pas etre confondu avec des fichiers generes.

## Notes de developpement

- Les dossiers `bin/`, `obj/`, `.vs/` et `TestResults/` sont des artefacts locaux et ne doivent pas etre versionnes.
- Les fichiers `.env`, secrets, chaines de connexion privees et resultats de tests doivent rester hors Git.
- Les tests de persistance ecrivent dans une base de donnees : utilisez une base dediee au test.
- Certains fichiers source contiennent actuellement des commentaires avec des caracteres accentues mal encodes. Cela n'empeche pas la compilation, mais une normalisation UTF-8 pourra etre utile plus tard.

## Commandes utiles

```bash
dotnet restore
dotnet build
dotnet test
dotnet test --filter "FullyQualifiedName!~PersistanceTests"
dotnet run --project TourneeFutee
```
