#include "colors.inc"
#include "textures.inc"
background { LightBlue }
camera {
  location <1, 0.3, 1>
  look_at <1, 0.3, 2> 
  angle 36
}
light_source { <0, 1, 0> White parallel} 

height_field{
      png
      "map.png"
      smooth
        texture{ Polished_Chrome }
  scale <10,0.5,10>
    }

sky_sphere {
    pigment {
      image_map{ png "skysphere.png"
                map_type 0    // planar
                interpolate 2 // bilinear
                once //
      }
      scale 2
      translate -1
    }
  }

#include "flock.pov"
