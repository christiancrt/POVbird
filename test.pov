#include "colors.inc"
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
        //pigment { Gray } 
        texture
    {
pigment
    {
    	Gray
       /* image_map
        {
            png "map.png"
        }*/
    }
    }
  scale <10,0.5,10>
    }

sky_sphere {
    pigment {
      image_map{ jpeg "early_in_the_morning_city_light_sky_sleep_3840x1080_hd-wallpaper-243496"
                map_type 0    // planar
                interpolate 2 // bilinear
                once //
      }
      scale 2
      translate -1
    }
  }

/*
// Macro for the adjustment of images
// for image_map with assumed_gamma = 1.0 ;
#macro Correct_Pigment_Gamma(Orig_Pig, New_G)
  #local Correct_Pig_fn =
      function{ pigment {Orig_Pig} }
  pigment{ average pigment_map{
   [function{ pow(Correct_Pig_fn(x,y,z).x, New_G)}
               color_map{[0 rgb 0][1 rgb<3,0,0>]}]
   [function{ pow(Correct_Pig_fn(x,y,z).y, New_G)}
               color_map{[0 rgb 0][1 rgb<0,3,0>]}]
   [function{ pow(Correct_Pig_fn(x,y,z).z, New_G)}
               color_map{[0 rgb 0][1 rgb<0,0,3>]}]
   }}
#end //
// "image_map" gamma corrected:
//    Correct_Pigment_Gamma(
//    pigment{ image_map{ jpeg "colors.jpg"}}
//    , Correct_Gamma)
//------------------------------------------------

box{ <-1, -1, -1>,< 1, 1, 1>
 texture{ uv_mapping
   Correct_Pigment_Gamma( // gamma correction
     pigment{
     image_map{ jpeg "early_in_the_morning_city_light_sky_sleep_3840x1080_hd-wallpaper-243496"
                map_type 0    // planar
                interpolate 2 // bilinear
                once //
              } //  end of image_map
    } // end of pigment
    , 2.2) //, New_Gamma
    finish { ambient 1 diffuse 0 }
 } // end of texture
scale 10000
} // end of skybox --------------------
*/