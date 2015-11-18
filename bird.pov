#include "transforms.inc"
#declare birdColor = pigment { rgb <1, 0, 0> };
#declare birdClock = clock;

// Construct the whole wing first, then cut it in half to obtain upper and lower wing parts
#declare wholeWing = 
difference {
	box {
		<.1, -.02, 0>, <-.1, .02, .6>
		pigment { birdColor }
		rotate <0, -10, 0>
	}
	box {
		<-.1, -.03, 0>, <-.2, .03, .6>
		pigment { birdColor }
		rotate <0, -4, 0>
	}
};

#declare upperWing =
difference {
	object {
		wholeWing
	}
	box {
		<.1, .1, .3>, <-.2, -.1, .7>
		pigment { birdColor }
	}
};

#declare lowerWing =
difference {
	object {
		wholeWing
	}
	box {
		<.1, .1, -.1>, <-.2, -.1, .3>
		pigment { birdColor }
	}
};

#declare birdHalf = union {
	// head
	cone {
		<.1, 0, 0>, 0.1, <.3, 0, 0>, 0.001
		pigment { birdColor }
		scale <0, 0.8, 0>
	}
	// body
	cylinder {
		<-.15, 0, 0>, <.1, 0, 0>, 0.1
		pigment { birdColor }
		scale <0, 0.8, 0>
	}
	// tail
	cone {
		<-.15, 0, 0>, .1, <-.35, 0, 0>, .04
		pigment { birdColor }
		scale <0, 0.8, 0>
	}
	difference {
		box {
			<-.2, -.02, 0>, <-.45, .02, .15>
			pigment { birdColor }
		}
		box {
			<0, -.1, 0>, <-.5, .1, .5>
			rotate <0, 15, 0>
			pigment { birdColor }
		}
	}
	// wing
	union {
		object {
			upperWing
		}
		object {
			lowerWing
			// lower wing movement			
			// clock factor changes flapping speed
			// sinus factor changes wing amplitude (in euler degrees)
			// additional +10 offsets wintip angle downwards
			Rotate_Around_Trans(<sin(clock * 60) * 25 + 10, 0, 0>, <0, 0, .3>)
		}
		// upper wing movement
		// We don't need Rotate_Around_Trans here as the lower wing is already
		// anchored in our origin.
		rotate <sin(clock * 60) * 30, 0, 0>
	}
};

// Constructing the bird from two halves saves animation (and modeling) work
#declare bird = union {
	object {
		birdHalf
	}
	object {
		birdHalf
		scale <1, 1, -1>
	}
	rotate <0, -90, 0>
};