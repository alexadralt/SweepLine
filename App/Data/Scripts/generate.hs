{-# LANGUAGE OverloadedStrings #-}
import Options.Applicative
import System.IO
import System.Directory
import System.Environment
import Data.List
import Control.Monad.Writer

-- ----------------------------------------------------------------------------
rMod :: Double -> Double -> Double
rMod a b = a - fromIntegral q * b
  where q = floor $ a / b

rMod2PI :: Double -> Double
rMod2PI a = rMod a (2*pi)

-- ----------------------------------------------------------------------------

data InputData = InputData
  { inFName :: String
  , outFName :: String
  } deriving (Show,Eq,Ord)

optionsParser :: Parser InputData
optionsParser = InputData
  <$> strOption
      ( long "input"
      <> short 'i'
      <> metavar "STRING"
      <> help "The name of file of scenario"
      )
  <*> strOption
      ( long "output"
      <> short 'o'
      <> metavar "STRING"
      <> help "The name of file to generate"
      )

-- ----------------------------------------------------------------------------
data Point = Point
  { x :: Double
  , y :: Double
  } deriving (Eq, Ord)

instance Show Point where
  show p = show (x p) ++ " " ++ show (y p)

instance Read Point where
  readsPrec _ input =
    [ (Point xc yc, rest)
    | ( xc, ' ':s1) <- reads input
    , ( yc, rest) <- reads $ drop 1 s1
    ]

str2point :: String -> String -> Point
str2point sx sy = Point (read sx) (read sy)

(+.) :: Point -> Point -> Point
p1 +. p2 = Point (x p1 + x p2) (y p1 + y p2)
infixl 6 +.

(-.) :: Point -> Point -> Point
p1 -. p2 = Point (x p1 - x p2) (y p1 - y p2)
infixl 6 -.

(*.) :: Double -> Point -> Point
a *. p = Point (a * x p) (a * y p)
infixl 7 *.

polarAngle :: Point -> Double
polarAngle (Point x y) = if temp < 0 then 2*pi + temp else temp
  where temp = atan2 y x

unitAngle :: Double -> Point
unitAngle a = Point (cos a) (sin a)

-- ----------------------------------------------------------------------------
data ArcDir = Counterclockwise | Clockwise
  deriving (Eq,Ord)

cwStr :: String
cwStr = "cw"

ccwStr :: String
ccwStr = "ccw"

instance Show ArcDir where
  show Counterclockwise = ccwStr
  show Clockwise = cwStr

instance Read ArcDir where
  readsPrec _ input
    | l == cwStr  = [(Clockwise, rest)]
    | l == ccwStr = [(Counterclockwise, rest)]
    | otherwise = error $ "Cannot parse ArcDir from string '" ++ input ++ "'"
    where ((l,rest):_) = lex input

-- ----------------------------------------------------------------------------
data ArcType = ShortArc | LongArc
  deriving (Eq,Ord)

shortArcStr :: String
shortArcStr = "short"

longArcStr :: String
longArcStr = "long"

instance Show ArcType where
  show ShortArc = shortArcStr
  show LongArc  = longArcStr

instance Read ArcType where
  readsPrec _ input
    | l == shortArcStr = [(ShortArc, rest)]
    | l == longArcStr  = [(LongArc, rest)]
    | otherwise = error $ "Cannot parse ArcType from string '" ++ input ++ "'"
    where ((l,rest):_) = lex input

-- ----------------------------------------------------------------------------

processLine :: Point -> Writer [Point] ()
processLine p = tell $ singleton p

processArc2p :: Point -> Point -> Double -> ArcType -> ArcDir -> Int -> Writer [Point] ()
processArc2p p1 p2 r arcType arcDir n = undefined

processArc3p :: Point -> Point -> Point -> Int -> Writer [Point] ()
processArc3p p1 p2 p3 n
  | abs d < 1e-6 = 
        error $ "Cannot construct anr by 3 points (" ++ show p1 ++ "), (" ++ show p1 ++ "), " ++ show p3 ++ "): they are located in a line!"
  | otherwise = mapM_ (\k -> tell $ singleton $ c +. r *. unitAngle (ang1 + fromIntegral k * da)) [1..n]
  where
    a1 = 2*(x p2 - x p1)
    b1 = 2*(y p2 - y p1)
    c1 = (x p2 ^ 2 - x p1 ^ 2) + (y p2 ^ 2 - y p1 ^ 2)
    a2 = 2*(x p3 - x p1)
    b2 = 2*(y p3 - y p1)
    c2 = (x p3 ^ 2 - x p1 ^ 2) + (y p3 ^ 2 - y p1 ^ 2)
    d = a1*b2 - b1*a2
    dx = c1*b2 - b1*c2
    dy = a1*c2 - c1*a2
    x0 = dx / d
    y0 = dy / d
    c = Point x0 y0
    r = sqrt $ (x p1 - x0)^2 + (y p1 - y0)^2
    ang1 = polarAngle (p1 -. c)
    ang2 = polarAngle (p1 -. c)
    ang3 = polarAngle (p3 -. c)
    da2 = rMod2PI (ang2 - ang1)
    da3 = rMod2PI (ang3 - ang1)
    da = (if da3 > da2 then da3 else da3 - 2*pi) / fromIntegral n

-- ----------------------------------------------------------------------------

lineCmd :: String
lineCmd = "lineto"

arc2pCmd :: String
arc2pCmd = "arc-2p"

arc3pCmd :: String
arc3pCmd = "arc-3p"

processCmd :: [String] -> Writer [Point] ()
processCmd (cmd:args)
  | cmd == lineCmd  = let (xs:ys:_) = args in processLine (str2point xs ys)
  | cmd == arc2pCmd = let (x1s:y1s:x2s:y2s:rs:lens:dirs:ns:_) = args in
                        processArc2p
                          (str2point x1s y1s)
                          (str2point x2s y2s)
                          (read rs)
                          (read lens)
                          (read dirs)
                          (read ns)
  | cmd == arc3pCmd = let (x1s:y1s:x2s:y2s:x3s:y3s:ns:_) = args in
                        processArc3p
                          (str2point x1s y1s)
                          (str2point x2s y2s)
                          (str2point x3s y3s)
                          (read ns)
  | otherwise = error $ "Unknown command '" ++ cmd ++ "'"

-- ----------------------------------------------------------------------------

main :: IO ()
main = do
    let
      optsInfo = info (optionsParser <**> helper)
        ( fullDesc
        <> progDesc "Generate a polyline according to the given scenario (see the file generate.txt)"
        <> header "generate - polyline generator" )
    opts <- execParser optsInfo
    cmds <- map words . lines <$> readFile (inFName opts)

    putStrLn $ "Processing " ++ show (length cmds) ++ " command(s)..."

    let
      pts = execWriter (mapM processCmd cmds)
      ptsStr = unlines $ map show pts

    writeFile (outFName opts) ptsStr

