#include "colors.inc"
background { LightBlue }
camera {
  location <1, 5, -5>
  look_at <0, 0, 1> 
  angle 36
}
light_source { <100, 500, -1000> White } 

height_field{
      png
      "map.png"
      smooth
        //pigment { Gray } 
        texture
    {
pigment
    {
        image_map
        {
            png "tex.png"
        }
    }
    }
  scale 2      
  translate -.5*x 
  rotate <-30, 0, 0 >
    }
