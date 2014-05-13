library(ggplot2)
speeds <- read.csv('path/to/file.csva')
ggplot(aes(x=Size, y=Duration, color=Type), data=speeds) + aes() +  scale_y_log10() + scale_x_log10() + geom_point() + facet_wrap( What ~ Operation ) + annotation_logticks(sides='bl')

