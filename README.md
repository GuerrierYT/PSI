# PSI
Projet de A2 S4

# Membre du groupe
* Alexandre LESUR
* Loris LIGONNIERE
* Antoine MASCIOTRA

Diagramme de classes :

```mermaid
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
    Program ..> ServicePersistance : utilise >
    Program ..> Little : utilise >
    
    ServicePersistance ..> Graph : sauvegarde/charge >
    ServicePersistance ..> Tour : sauvegarde/charge >
    ServicePersistance ..> Matrix : accède aux données >

    Graph "1" *-- "*" Vertex : contient >
    Graph "1" *-- "1" Matrix : possède >
    Vertex "*" -- "*" Vertex : voisins >
    
    Little "1" o-- "1" Graph : manipule >
    Little ..> Tour : produit >
    Little ..> LittleNoeud : instancie >
    
    LittleNoeud "1" o-- "1" Matrix : contient >
