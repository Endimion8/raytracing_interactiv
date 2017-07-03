#version 130
uniform float aspect ;
uniform vec3 campos;
in vec3 vPosition; //¬ходные переменные vPosition - позци€ вершины


out vec3 org, dir;
void main() 
{ 
 
	gl_Position = vec4(vPosition, 1.0);
	dir = normalize(vec3(vPosition.x*aspect, vPosition.y, -1.0));
	org = campos;
}