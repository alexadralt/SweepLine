{-# LANGUAGE OverloadedStrings #-}
import Options.Applicative
import System.IO
import System.Environment
import Data.List

data InputData = InputData
  { r :: Double
  , cX :: Double
  , cY :: Double
  , vQnt :: Int
  , outFName :: String
  } deriving (Show,Eq,Ord)

optionsParser :: Parser InputData
optionsParser = InputData
  <$> option auto
      ( long "radius"
     <> short 'r'
     <> metavar "DOUBLE"
     <> help "The radius of the circle" )
  <*> option auto
      ( long "cx"
     <> short 'x'
     <> metavar "DOUBLE"
     <> help "The abscissa of the center" )
  <*> option auto
      ( long "cy"
     <> short 'y'
     <> metavar "DOUBLE"
     <> help "The ordinate of the center" )
  <*> option auto
      ( long "vertnum"
     <> short 'n'
     <> metavar "INT"
     <> help "The number of vertices" )
  <*> strOption
      ( long "output"
     <> short 'o'
     <> metavar "STRING"
     <> help "The name of file to generate" )

main :: IO ()
main = do
    let
      optsInfo = info (optionsParser <**> helper)
        ( fullDesc
        <> progDesc "Generate a right polygon approximating a circle to a text file"
        <> header "_do_circle - circle generator" )
    opts <- execParser optsInfo
    let
        da = 2*pi / fromIntegral (vQnt opts)
        coords = map (\a -> (r opts * cos a + cX opts, r opts * sin a + cX opts) ) $ init [0, da .. (2*pi)]

        coordsStr =
            concat $
            intersperse "\n" $
            map (\(x,y) -> show x ++ " " ++ show y) coords

    writeFile (outFName opts) coordsStr
