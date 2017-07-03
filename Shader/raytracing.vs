#version 440 

uniform float aspect ;
uniform vec3 campos;
uniform vec3 sphereCenter;
uniform vec3 lposition;

in vec3 vPosition; //Входные переменные vPosition - позиция вершины
out vec3 camPos, sc, lpos, glPosition;  


void main (void) 
{    
	gl_Position = vec4(vPosition, 1.0);   
	glPosition = vPosition; 
	camPos = campos;
	sc = sphereCenter;
	lpos = lposition;
}



