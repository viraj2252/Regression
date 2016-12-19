
#r "../packages/Accord.3.3.0/lib/net45/Accord.dll"
#r "../packages/Accord.Statistics.3.3.0/lib/net45/Accord.Statistics.dll"
#r "../packages/Accord.Math.3.3.0/lib/net45/Accord.Math.dll"

let input = [|1,1.;2,2.;3,2.25;4,4.75;5,5.|]

//#region Accord

open Accord
open Accord.Statistics.Models.Regression.Linear

let x = input |> Array.map (fun (x,y) -> float x)
let y = input |> Array.map (fun (x,y) -> y)

let ols = OrdinaryLeastSquares()
let regression = ols.Learn(x,y)
let intercept = regression.Intercept
let slope = regression.Slope
let r2 = regression.CoefficientOfDetermination(x,y)

//#endregion

//#region Region Name

let variance (source:float seq) = 
    let mean = Seq.average source
    let deltas = Seq.map (fun x -> pown (x - mean) 2) source
    Seq.average deltas

let standardDeviation values =
    sqrt (variance values)

let x = input |> Seq.map (fun (x,y) -> float x)
let y = input |> Seq.map (fun (x,y) -> y)

let mX = Seq.average x
let mY = Seq.average y

let sX = standardDeviation x
let sY = standardDeviation y

let pearsonsCorrelation (a:float seq, b:float seq) = 
    let mX = Seq.average a
    let mY = Seq.average b

    let x = a |> Seq.map (fun x -> x - mX)
    let y = b |> Seq.map (fun y -> y - mY)

    let xys = Seq.zip x y
    let xy = xys |> Seq.map (fun (x, y) -> x*y, x*x, y*y)
    let sxy = xy |> Seq.sumBy (fun (xy, x2, y2) -> xy)
    let sx2 = xy |> Seq.sumBy (fun (xy, x2, y2) -> x2)
    let sy2 = xy |> Seq.sumBy (fun (xy, x2, y2) -> y2)

    sxy / sqrt (sx2*sy2)


let r = pearsonsCorrelation (x, y)
//slope
let b = r*(sY/sX)
//Intercept A
let A = mY - b*mX

//#endregion
