global_settings { assumed_gamma 1 }

camera {
   orthographic
   location <0, 0, -1>
   look_at <0, 0, 0>
   right  x*image_width/image_height
}

#declare DT_AO     = 1;  // the ambient occlusion map
#declare DT_Normal = 2;  // the normal render map
#declare DT_Mult   = 3;  // the multiplied AO*Normal map

//#declare Display_Type = DT_AO;
//#declare Display_Type = DT_Normal;
#declare Display_Type = DT_Mult;

#declare p1 = pigment {
  granite
   translate -0.5
}
#declare p2 = pigment {
   granite
   translate -0.5
}

#declare fp1 = function { pigment { p1 } };
#declare fp2 = function { pigment { p2 } };

#declare RED = pigment {
   function { fp1(x,y,z).red * fp2(x,y,z).red }
   color_map { [0 rgb 0][1 rgb <1,0,0>] }
}

#declare GREEN = pigment {
   function { fp1(x,y,z).green * fp2(x,y,z).green }
   color_map { [0 rgb 0][1 rgb <0,1,0>] }
}

#declare BLUE = pigment {
   function { fp1(x,y,z).blue * fp2(x,y,z).blue }
   color_map { [0 rgb 0][1 rgb <0,0,1>] }
}

#declare p3 = pigment {
   average
   pigment_map {
      [1 RED]
      [1 GREEN]
      [1 BLUE]
   }
}

plane { z, 0
   hollow
   #switch (Display_Type)
      #case (DT_AO)
         pigment { p1 }
         finish { ambient 1 diffuse 0 }
         #break
      #case (DT_Normal)
         pigment { p2 }
         finish { ambient 1 diffuse 0 }
         #break
      #case (DT_Mult)
         pigment { p3 }
         finish { ambient 3 diffuse 0 }
         #break
   #end
}