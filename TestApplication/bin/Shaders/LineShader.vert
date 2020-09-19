#version 400    core

layout (location = 0) in vec2 aPosition;

uniform mat4 transform;

void main()
{
    gl_Position = transform * vec4(aPosition, 0.0, 1.0);
}