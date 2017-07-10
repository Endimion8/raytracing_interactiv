﻿#version 440  

out vec4 FragColor; 
in vec3 camPos, sc, lpos, glPosition;  

#define EPSILON 0.001
#define BIG 1000000.0
const int DIFFUSE = 1;
const int REFLECTION = 2;
const int REFRACTION = 3;

const int DIFFUSE_REFLECTION = 1;	//	пересечение с диффузным объектом
const int MIRROR_REFLECTION = 2;	// пересечение с зеркальным объектом

const int lightNum= 1;

vec3 v = vec3(2, 1, 2);
<<<<<<< HEAD
=======
vec3 vvv = vec3(2, 1, 2);
>>>>>>> master

/*** DATA STRUCTURES ***/
struct SCamera
{
	vec3 Position;
	vec3 View;
	vec3 Up;
	vec3 Side;
	// отношение сторон выходного изображения
	vec2 Scale;
};

struct SRay
{
	vec3 Origin;
	vec3 Direction;
};

struct SSphere
{
    vec3 Center;
    float Radius;
    int MaterialIdx;
};

struct STriangle
{
    vec3 v1;
    vec3 v2;
    vec3 v3;
    int MaterialIdx;
};
struct SLight
{
	vec3 Position;
};

struct SMaterial
{
    //diffuse color
	vec3 Color;
    // ambient, diffuse and specular coeffs
	vec4 LightCoeffs;
    // 0 - non-reflection, 1 - mirror
	float ReflectionCoef;
    float RefractionCoef;
    int MaterialType;
};

struct SIntersection
{
	float Time;
	vec3 Point;
	vec3 Normal;
	vec3 Color;

	vec4 LightCoeffs;

	float ReflectionCoef;
	float RefractionCoef;
	int MaterialType;
};

struct STracingRay
{
	SRay ray; // луч
	float contribution;	// вклад луча в результирующий цвет
	int depth; // номер переотражения
};

SRay GenerateRay ( SCamera uCamera )
{
    vec2 coords = glPosition.xy * uCamera.Scale;
    vec3 direction = uCamera.View + uCamera.Side * coords.x + uCamera.Up * coords.y;
    return SRay ( uCamera.Position, normalize(direction) );
}


SCamera initializeDefaultCamera()
{
    //** CAMERA **//
	SCamera camera;
	camera.Position = camPos; //vec3(0.0, 0.0, -8);
    camera.View = vec3(0.0, 0.0, 1.0);
    camera.Up = vec3(0.0, 1.0, 0.0);
    camera.Side = vec3(1.0, 0.0, 0.0);
    camera.Scale = vec2(1.0);
	return camera;
}



void initializeDefaultScene( out STriangle triangles[12],  out SSphere spheres[3])
{
	/** TRIANGLES **/
	
	/* left wall */
	triangles[0].v1 = vec3(-5.0,-5.0,-8.1);
	triangles[0].v2 = vec3(-5.0, 5.0, 5.0);
	triangles[0].v3 = vec3(-5.0, 5.0,-8.1);
	triangles[0].MaterialIdx = 0;
	triangles[1].v1 = vec3(-5.0,-5.0,-8.1);
	triangles[1].v2 = vec3(-5.0,-5.0, 5.0);
	triangles[1].v3 = vec3(-5.0, 5.0, 5.0);
	triangles[1].MaterialIdx = 0;
	
	/* back wall */
	triangles[2].v1 = vec3(-5.0,-5.0, 5.0);
	triangles[2].v2 = vec3( 5.0,-5.0, 5.0);
	triangles[2].v3 = vec3(-5.0, 5.0, 5.0);
	triangles[2].MaterialIdx = 1;
	triangles[3].v1 = vec3( 5.0, 5.0, 5.0);
	triangles[3].v2 = vec3(-5.0, 5.0, 5.0);
	triangles[3].v3 = vec3( 5.0,-5.0, 5.0);
	triangles[3].MaterialIdx = 1;
	
	/*right wall */
	triangles[4].v1 = vec3(5.0, 5.0, 5.0);
	triangles[4].v2 = vec3(5.0, -5.0, 5.0);
	triangles[4].v3 = vec3(5.0, 5.0, -8.1);			
	triangles[4].MaterialIdx = 2;
	triangles[5].v1 = vec3(5.0, 5.0, -8.1);
	triangles[5].v2 = vec3(5.0, -5.0, 5.0);
	triangles[5].v3 = vec3(5.0, -5.0, -8.1);				
	triangles[5].MaterialIdx = 2;
	
	/*down wall */
	triangles[6].v1 = vec3(-5.0,-5.0, 5.0);
	triangles[6].v2 = vec3(-5.0,-5.0,-8.1);
	triangles[6].v3 = vec3( 5.0,-5.0, 5.0);
	triangles[6].MaterialIdx = 3;
	triangles[7].v1 = vec3(5.0, -5.0, -8.1);
	triangles[7].v2 = vec3(5.0,-5.0, 5.0);
	triangles[7].v3 = vec3(-5.0,-5.0,-8.1);
	triangles[7].MaterialIdx = 3;
	
	/*up wall */
	triangles[8].v1 = vec3(-5.0, 5.0,-8.1);
	triangles[8].v2 = vec3(-5.0, 5.0, 5.0);
	triangles[8].v3 = vec3( 5.0, 5.0, 5.0);
	triangles[8].MaterialIdx = 4;
	triangles[9].v1 = vec3(-5.0, 5.0, -8.1);
	triangles[9].v2 = vec3( 5.0, 5.0, 5.0);
	triangles[9].v3 = vec3(5.0, 5.0, -8.1);
	triangles[9].MaterialIdx = 4;

	/*front wall*/
	triangles[10].v1 = vec3(-5.0,-5.0, -8.1);
	triangles[10].v2 = vec3( 5.0,-5.0, -8.1);
	triangles[10].v3 = vec3(-5.0, 5.0, -8.1);
	triangles[10].MaterialIdx = 5;
	triangles[11].v1 = vec3( 5.0, 5.0, -8.1);
	triangles[11].v2 = vec3(-5.0, 5.0, -8.1);
	triangles[11].v3 = vec3( 5.0,-5.0, -8.1);
	triangles[11].MaterialIdx = 5;
	
	/** SPHERES **/
	spheres[0].Center = vec3(-1.0,-1.0,-1.0);
	spheres[0].Radius =  1.5;
	spheres[0].MaterialIdx = 2;
	spheres[1].Center = vec3(2, 1, 2);
	spheres[1].Radius = 1.0; 
	spheres[1].MaterialIdx = 3;
    spheres[2].Center = sc; //vec3(-2, 1, 2);
	spheres[2].Radius = 0.5; 
	spheres[2].MaterialIdx = 5;
}

bool IntersectSphere ( SSphere sphere, SRay ray, float start, float final, out float time )
{
	ray.Origin -= sphere.Center;
	float A = dot ( ray.Direction, ray.Direction );
	float B = dot ( ray.Direction, ray.Origin );
	float C = dot ( ray.Origin, ray.Origin ) - sphere.Radius * sphere.Radius;
	float D = B * B - A * C;
	if ( D > 0.0 )
	{
		D = sqrt(D);
		//time = min(max(0, ( -B - D ) / A ), ( -B + D ) / A );
		float t1 = ( -B - D ) / A;
		float t2 = ( -B + D ) / A;
		if((t1 < 0) && (t2 < 0))
			return false;

		if(min(t1, t2) < 0)
		{
			time = max(t1,t2);
			return true;
		}
		time = min(t1, t2);
		return true;
	}
	return false;
}

bool IntersectTriangle (SRay ray, vec3 v1, vec3 v2, vec3 v3, out float time )
{
    // // Compute the intersection of ray with a triangle using geometric solution
	// Input: // points v0, v1, v2 are the triangle's vertices
	// rayOrig and rayDir are the ray's origin (point) and the ray's direction
	// Return: // return true is the ray intersects the triangle, false otherwise
	// bool intersectTriangle(point v0, point v1, point v2, point rayOrig, vector rayDir)
        // compute plane's normal vector
	time = -1;
    vec3 A = v2 - v1;
    vec3 B = v3 - v1;
    // no need to normalize vector
	vec3 N = cross(A, B);
    // // Step 1: finding P
	// // check if ray and plane are parallel ?
	float NdotRayDirection = dot(N, ray.Direction);
    if (abs(NdotRayDirection) < 0.001)
		return false;
    // they are parallel so they don't intersect !
	// compute d parameter using equation 2
	float d = dot(N, v1);
    // compute t (equation 3)
	float t = -(dot(N, ray.Origin) - d) / NdotRayDirection;
    // check if the triangle is in behind the ray
	if (t < 0)	
		return false;
    // the triangle is behind
	// compute the intersection point using equation 1
	vec3 P = ray.Origin + t * ray.Direction;
    // // Step 2: inside-outside test //
	vec3 C;
    // vector perpendicular to triangle's plane
	//edge 0
	vec3 edge1 = v2 - v1;
    vec3 VP1 = P - v1;
    C = cross(edge1, VP1);
    if (dot(N, C) < 0)
		return false;
    // P is on the right side
	// edge 1
	vec3 edge2 = v3 - v2;
    vec3 VP2 = P - v2;
    C = cross(edge2, VP2);
    if (dot(N, C) < 0)
		return false;
    // P is on the right side
	// edge 2
	vec3 edge3 = v1 - v3;
    vec3 VP3 = P - v3;
    C = cross(edge3, VP3);
    if (dot(N, C) < 0)
		return false;
    // P is on the right side;
	time = t;
    return true;
    // this ray hits the triangle
}



bool Raytrace ( SRay ray, SSphere spheres[3], STriangle triangles[12], SMaterial materials[6], float start, float final, inout SIntersection intersect )
{
	bool result = false;
	float test = start;
	intersect.Time = final;
	for(int i = 0; i < 3; i++)
	{
		SSphere sphere = spheres[i];
		if( (IntersectSphere(sphere, ray, start, final, test)) && (test < intersect.Time) )
		{
			intersect.Time = test;
			intersect.Point = ray.Origin + ray.Direction * test;
			intersect.Normal = normalize ( intersect.Point - spheres[i].Center );
			intersect.Color = materials[sphere.MaterialIdx].Color;
			intersect.LightCoeffs = materials[sphere.MaterialIdx].LightCoeffs;
			intersect.ReflectionCoef = materials[sphere.MaterialIdx].ReflectionCoef;
			intersect.RefractionCoef = materials[sphere.MaterialIdx].RefractionCoef;
			intersect.MaterialType = materials[sphere.MaterialIdx].MaterialType;
			result = true;
		}
	}

	for(int i = 0; i < 12; i++)
	{
		STriangle triangle = triangles[i];
		if((IntersectTriangle(ray, triangle.v1, triangle.v2, triangle.v3, test)) && (test < intersect.Time))
		{ 
			intersect.Time = test;
			intersect.Point = ray.Origin + ray.Direction * test;
			intersect.Normal = normalize(cross(triangle.v1 - triangle.v2, triangle.v3 - triangle.v2));
			intersect.Color = materials[triangle.MaterialIdx].Color;
			intersect.LightCoeffs = materials[triangle.MaterialIdx].LightCoeffs;
			intersect.ReflectionCoef = materials[triangle.MaterialIdx].ReflectionCoef;
			intersect.RefractionCoef = materials[triangle.MaterialIdx].RefractionCoef;
			intersect.MaterialType = materials[triangle.MaterialIdx].MaterialType;
			result = true;
		}
	}
	return result;
}

void initializeDefaultLightMaterials(out SLight light[lightNum], out SMaterial materials[6])
{
	//** LIGHT **//
//    for (int i = 0; i< 10; i++)
//        for (int j = 0; j< 10; j++)
//        {
//	       light[i*10 + j].Position = lpos + vec3(EPSILON*j, EPSILON*i, 0.0 ); //vec3(0.0, 2.0, -4.0);
//         }
       
       for(int i=0; i<lightNum; i++)
       light[i].Position = vec3(0.0+sin(i)*0.3, 2.0+cos(i)*0.3, -4.0);

	//light.Position = vec3(0.0, 4.0, 0.0);
	/** MATERIALS **/
	vec4 lightCoefs = vec4(0.4 / lightNum,0.9 / lightNum,0.0,512.0);

	materials[0].Color = vec3(0.0, 1.0, 0.0);
	materials[0].LightCoeffs = vec4(lightCoefs);
	materials[0].ReflectionCoef = 0.5;
	materials[0].RefractionCoef = 1.0;
	materials[0].MaterialType = DIFFUSE;
	
	materials[1].Color = vec3(0.0, 0.3, 1.0);
	materials[1].LightCoeffs = vec4(lightCoefs);
	materials[1].ReflectionCoef = 0.5;
	materials[1].RefractionCoef = 1.0;
	materials[1].MaterialType = DIFFUSE;
	
	materials[2].Color = vec3(0.0, 0.0, 0.0);
	materials[2].LightCoeffs = vec4(lightCoefs);
	materials[2].ReflectionCoef = 0.5;
	materials[2].RefractionCoef = 1.0;
	materials[2].MaterialType = MIRROR_REFLECTION;
	
	materials[3].Color = vec3(1.0, 1.0, 1.0);
	materials[3].LightCoeffs = vec4(lightCoefs);
	materials[3].ReflectionCoef = 0.5;
	materials[3].RefractionCoef = 1.0;
	materials[3].MaterialType = MIRROR_REFLECTION;
	
	materials[4].Color = vec3(1.0, 1.0, 1.0);
	materials[4].LightCoeffs = vec4(lightCoefs);
	materials[4].ReflectionCoef = 0.5;
	materials[4].RefractionCoef = 1.0;
	materials[4].MaterialType = DIFFUSE_REFLECTION;
	
	materials[5].Color = vec3(0, 1.0, 1.0);
	materials[5].LightCoeffs = vec4(lightCoefs);
	materials[5].ReflectionCoef = 0.5;
	materials[5].RefractionCoef = 1.0;
	materials[5].MaterialType = DIFFUSE_REFLECTION;	

}
vec3 Phong ( SCamera uCamera, SIntersection intersect, SLight currLight, float shadowing)
{
	vec3 light = normalize ( currLight.Position - intersect.Point );
	float diffuse = max(dot(light, intersect.Normal), 0.0);
	vec3 view = normalize(uCamera.Position - intersect.Point);
	vec3 reflected = reflect( -view, intersect.Normal );
	float specular = pow(max(dot(reflected, light), 0.0), intersect.LightCoeffs.w);
	int Unit = 1; // яркость бликов
	return intersect.LightCoeffs.x * intersect.Color + 
		   intersect.LightCoeffs.y * diffuse * intersect.Color * shadowing  + 
		   intersect.LightCoeffs.z * specular * Unit;
}

float Shadow(SLight currLight, SIntersection intersect, SSphere spheres[3], STriangle triangles[12], SMaterial materials[6])
{

	float shadowing = 1.0;
	vec3 direction = normalize(currLight.Position - intersect.Point);
	float distanceLight = distance(currLight.Position, intersect.Point);
	SRay shadowRay = SRay(intersect.Point + direction * EPSILON, direction);
	SIntersection shadowIntersect;
	shadowIntersect.Time = BIG;
	if(Raytrace(shadowRay, spheres, triangles, materials, 0.0, distanceLight, shadowIntersect))
	{
		shadowing = 0.0;
    }
	return shadowing;
}

const int N = 12;
STracingRay t[N];
int counter = 0;
void pushRay(STracingRay el) // положить луч в стек
{
	if (counter < N) t[counter++] = el;
}
STracingRay popRay() // взять луч из стека
{
	int temp = counter;
	if (counter > 0) counter--; // если счетчик больше 0, то уменьшаем его на 1
	return t[counter];
}
bool isEmpty()	// проверка пустоты
{
	return counter == 0;
}



void main ( void ) 
{   
	float start = 0;
	float final = BIG;
	//float contribution = 0.5;

	STriangle triangles[12];
	SSphere spheres[3];
	SLight light[lightNum];
	SMaterial materials[6];
	vec3 resultColor = vec3(0,0,0);

	SCamera uCamera = initializeDefaultCamera();
	SRay ray = GenerateRay( uCamera);
	STracingRay trRay = STracingRay(ray, 1, 0);

	initializeDefaultScene(triangles, spheres);
	initializeDefaultLightMaterials(light, materials);

	SIntersection intersect;
	pushRay(trRay);
	while(!isEmpty())
	{
		STracingRay trRay = popRay();
		ray = trRay.ray;
		intersect.Time = BIG;
		start = 0;
		final = BIG;
		if (Raytrace(ray, spheres, triangles, materials, start, final, intersect))	// вычисление освещения
		{
			switch(intersect.MaterialType)
			{
			case DIFFUSE_REFLECTION:
			{
            float shadowing = 0;
             for (int i = 0; i< lightNum; i++)
                {
				shadowing = Shadow(light[i], intersect, spheres, triangles, materials); //в методичке неправильно
				resultColor += trRay.contribution* 1 * Phong (uCamera,intersect, light[i], shadowing ); 
                }
				break;
			}
			case MIRROR_REFLECTION:
			{
				if (intersect.ReflectionCoef < 1)
				{
					float contribution = trRay.contribution * (1 - intersect.ReflectionCoef);
					//contribution *= intersect.ReflectionCoef;
                    float shadowing = 0;
                    for (int i = 0; i< lightNum; i++)
                    {
					shadowing = Shadow(light[i], intersect, spheres, triangles, materials);
					resultColor += trRay.contribution * 1 * Phong (uCamera,intersect, light[i], shadowing );   // уменьшил интенсивность отраженного света вполовину
                    }
				}
				vec3 reflectDirection = reflect(ray.Direction, intersect.Normal);
				// создать луч отражения
				float contribution = trRay.contribution * intersect.ReflectionCoef;
				//float contribution *= intersect.ReflectionCoef;
				STracingRay reflectRay = STracingRay(SRay(intersect.Point + reflectDirection * EPSILON, reflectDirection),contribution, trRay.depth + 1);
				pushRay(reflectRay);
				break;
			}
			}
		}
	}
	FragColor = vec4 (resultColor, 1.0);
}