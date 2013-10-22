using System;
using System.IO;
using System.Linq;
using RDotNet;

namespace Diagnose
{
    class Program
    {
        private static void Main(string[] args)
        {
            var envPath = Environment.GetEnvironmentVariable("PATH");
            var rBinPath = @"F:\bin\R\bin\x64";
            Environment.SetEnvironmentVariable("PATH", envPath + Path.PathSeparator + rBinPath);
            using (REngine engine = REngine.CreateInstance("RDotNet"))
            {
                engine.Initialize();
                engine.Evaluate("library(flexclust)");
                engine.Evaluate("set.seed(42)");
                engine.Evaluate("g1 <- sample(1:5, size=1000, replace=TRUE)");
                engine.Evaluate("g2 <- sample(1:5, size=1000, replace=TRUE)");
                engine.Evaluate("tab <- table(g1, g2)");
                double result = engine.Evaluate("randIndex(tab,correct=FALSE)").AsNumeric().First();
                TestMailingList(engine);
            }
        }

        private const string fplot = @"makeProfilePlot <- function(mylist,names){
  require(RColorBrewer);
  numvariables <- length(mylist);     
  colours <- brewer.pal(numvariables,'Set1');                
  mymin <- 1e+20;
  mymax <- 1e-20;
  for (i in 1:numvariables)
  {
    vectori <- mylist[[i]];
    mini <- min(vectori);
    maxi <- max(vectori);
    mymin <- mini;              
    mymax <- maxi;
    if (mini < mymin) { mymin <- mini};
    if (maxi > mymax) { mymax <- maxi};
  };
  png('f:/tmp/test_plot.png');
  for (i in 1:numvariables)
  {
    vectori <- mylist[[i]];
    namei <- names[i];
    colouri <- colours[i];
    if (i == 1) { plot(vectori,col=colouri,type='l',ylim=c(mymin,mymax)) }
    else   { points(vectori, col=colouri,type='l') };
    lastxval <- length(vectori);
    lastyval <- vectori[length(vectori)];
    text((lastxval-10),(lastyval),namei,col='black',cex=0.6);       
  };
  dev.off();
}";
        private static void TestMailingList(REngine engine)
        {
            Function makeProfilePlot = engine.Evaluate(fplot).AsFunction();
            engine.Evaluate("library(RColorBrewer)");
            engine.Evaluate("d <- as.data.frame(matrix(rnorm(600), ncol=6))");
            engine.Evaluate("dnames <- c('V2','V3','V4','V5','V6')         ");
            engine.Evaluate("names(d) <- dnames                            ");
            engine.Evaluate("makeProfilePlot(d,dnames)                     ");
        }
    }
}
