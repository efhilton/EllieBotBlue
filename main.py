import motors as m
import brain as b

motors = m.Motors()
brain = b.Brain(motors)

brain.parse_string("f:0.5,r:1.2:s:1.0,f:0.2,l:0.1,b:1.2,s:1.0")
brain.parse_string("F:0.5,R:1.2:S:1.0,f:0.2,l:0.1,b:1.2,s:1.0")
