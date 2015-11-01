from vapory import *
from moviepy.editor import VideoClip # For animation

# Don't forget to add objects to the objects array!
def scene(t):
	light = LightSource([10, 15, -20], [1.3, 1.3, 1.3])
	wall = Plane([0, 0, 1], 20, Texture(Pigment('color', [1, 1, 1])))
	ground = Plane([0, 1, 0], 0,
		Texture( Pigment( 'color', [1, 1, 1]),
			Finish( 'phong', 0.1, 'reflection',0.4, 'metallic', 0.3)))

	sphere1 = Sphere([-4, 2, 2], 2.0,
		Pigment('color', [0, 0, 1]),
		# ImageMap(image = "map.png"),
			Finish('phong', 0.8, 'reflection', 0.5))
	sphere2 = Sphere([-4*t, 2, 0], 1.7,
		Texture('T_Ruby_Glass'),
		Interior('ior',2))
	diff = Difference(sphere1, sphere2)

	# heightfield = HeightField(heightfield = "map.png", smooth=True,
	# 	pigment = Texture(Pigment([1, 0, .2])))

	return Scene( Camera("location", [0, 5, -10], "look_at", [1, 3, 0]),
		objects = [ground, wall, light, diff],
		atmospheric = [], # fog et al.
		included = ["glass.inc", "colors.inc"])
#scene.render("out.png", width=1024, height=768)

def make_frame(t):
    return scene(t).render(width = 300, height = 200,
    	antialiasing = 0.001,
    	quality = 10, # 1 - 10
    	includedirs = ["/Users/marcel/uhh/ivc/PovrayCommandLineMacV2/include/"])

VideoClip(make_frame, duration = 2).write_gif("anim.gif",fps = 20)
