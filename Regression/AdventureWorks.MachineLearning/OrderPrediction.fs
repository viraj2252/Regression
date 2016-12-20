namespace AdventureWorks.MachineLearning

open Accord
open Accord.Statistics
open Accord.Statistics.Models.Regression.Linear

open System
open System.Data.SqlClient
open System.Collections.Generic

type internal ProductReview = {ProductID:int; AvgOrders:float; AvgReviews: float}

type public OrderPrediction () = 
    let reviews = List<ProductReview>()

    [<Literal>]
    let connectionString = "Data Source=localhost;Initial Catalog=AdventureWorks2014;Integrated Security=True"

    [<Literal>]
    let query = "Select 
                A.ProductID, AvgOrders, AvgReviews
                From
                (Select 
                ProductID,
                (Sum(OrderQty) + 0.0)/(Count(Distinct SOH.CustomerID) + 0.0) as AvgOrders
                from [Sales].[SalesOrderDetail] as SOD
                inner join [Sales].[SalesOrderHeader] as SOH
                on SOD.SalesOrderID = SOH.SalesOrderID
                inner join [Sales].[Customer] as C
                on SOH.CustomerID = C.CustomerID
                Where C.StoreID is not null
                Group By ProductID) as A
                Inner Join 
                (Select
                ProductID,
                (Sum(Rating) + 0.0) / (Count(ProductID) + 0.0) as AvgReviews
                from [Production].[ProductReview] as PR
                Group By ProductID) as B
                on A.ProductID = B.ProductID
                "

    member this.PredictQuantity(productId:int) = 
        use connection = new SqlConnection(connectionString)
        use command = new SqlCommand(query,connection)
        connection.Open()
        use reader = command.ExecuteReader()
        while reader.Read() do
            reviews.Add({ProductID=reader.GetInt32(0);AvgOrders=(float)(reader.GetDecimal(1));AvgReviews=(float)(reader.GetDecimal(2))})

        let x = reviews |> Seq.map(fun pr -> pr.AvgReviews) |> Seq.toArray
        let y = reviews |> Seq.map(fun pr -> pr.AvgOrders) |> Seq.toArray
        let regression = SimpleLinearRegression()
        let sse = regression.Regress(x,y)
        let mse = sse/float x.Length 
        let rmse = sqrt(mse)
        let r2 = regression.CoefficientOfDetermination(x,y)

        let review = reviews |> Seq.find(fun r -> r.ProductID = productId)
        regression.Compute(review.AvgReviews)
    

