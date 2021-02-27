using System;
using System.Collections.Generic;

namespace whistlehead_Roadways_V2_GUI
{

    public class splineData
    {
        public List<sectData> edges;
        public List<sectData> lanes;
        public List<sectData> lines;
        public List<aiPath> paths;

        public splineData()
        {
            edges = new List<sectData>();
            lanes = new List<sectData>();
            lines = new List<sectData>();
            paths = new List<aiPath>();
        }

        public int Count()
        {
            return edges.Count + lanes.Count + lines.Count + paths.Count;
        }
    }

    public class sectData
    {
        public double offset;
        public bool mirror;
        public string mat;
        public bool mat_override = false;

        public List<profData> profiles;

        public sectData(List<profData> in_profiles, double in_offset, bool in_mirror = false)
        {
            profiles = in_profiles;
            offset = in_offset;
            mirror = in_mirror;
        }

        public sectData(List<profData> in_profiles, bool in_mirror = false)
        {
            profiles = in_profiles;
            offset = 0;
            mirror = in_mirror;
        }

        public sectData(List<profData> in_profiles, string in_mat, double in_offset, bool in_mirror = false)
        {
            profiles = in_profiles;
            offset = in_offset;
            mirror = in_mirror;
            mat = in_mat;
            mat_override = true;
        }

        public sectData(List<profData> in_profiles, string in_mat, bool in_mirror = false)
        {
            profiles = in_profiles;
            offset = 0;
            mirror = in_mirror;
            mat = in_mat;
            mat_override = true;
        }
    }

    public struct aiPath
    {
        public int type;
        public double offset;
        public double height;
        public double width;
        public int direction;

        public aiPath(ePathType in_type, double in_offset, double in_height, double in_width, int in_direction)
        {
            type = (int)in_type;
            offset = in_offset;
            height = in_height;
            width = in_width;
            direction = in_direction;
        }
    }

    public class lineMetaData
    {
        private List<profData> prof;
        public string longName { get; private set; }
        private string shortName;

        public lineMetaData(string in_longName, string in_shortName, List<profData> in_prof)
        {
            prof = in_prof;
            longName = in_longName;
            shortName = in_shortName;
        }

        public lineMetaData(string in_longName, string in_shortName, profData in_prof)
        {
            prof = new List<profData> { in_prof };
            longName = in_longName;
            shortName = in_shortName;
        }

        public List<profData> Prof()
        {
            return prof;
        }

        public string LongName()
        {
            return longName;
        }

        public string ShortName()
        {
            return shortName;
        }
    }

    public class profData
    {
        public int len;
        public string mat;
        public double[] pnts;
        public double[] hgts;
        public double[] texs;
        public double[] reps;

        public profData(int in_len, string in_mat, double[] in_pnts, double[] in_hgts, double[] in_texs, double[] in_reps)
        {
            len = in_len;
            mat = in_mat;

            pnts = fillArray(in_pnts, in_len);
            hgts = fillArray(in_hgts, in_len);
            texs = fillArray(in_texs, in_len);
            reps = fillArray(in_reps, in_len);
        }

        public profData(int in_len, string in_mat, double[] in_pnts, double in_hgt, double[] in_texs, double in_rep)
        {
            len = in_len;
            mat = in_mat;

            pnts = fillArray(in_pnts, in_len);
            hgts = fillArray(new double[] { in_hgt }, in_len);
            texs = fillArray(in_texs, in_len);
            reps = fillArray(new double[] { in_rep }, in_len);
        }

        public profData(int in_len, string in_mat, double[] in_pnts, double[] in_hgts, double[] in_texs, double in_rep)
        {
            len = in_len;
            mat = in_mat;

            pnts = fillArray(in_pnts, in_len);
            hgts = fillArray(in_hgts, in_len);
            texs = fillArray(in_texs, in_len);
            reps = fillArray(new double[] { in_rep }, in_len);
        }

        public static double[] fillArray(double[] in_arr, int in_len)
        {
            // protection against non-allowable 1 value profiles
            int len = Math.Max(in_len, 2);

            double[] arr = new double[len];
            int in_arr_len = in_arr.Length;
            for (int i = 0; i < len; i++)
            {
                // if i exists, copy it
                if (i < in_arr_len)
                {
                    arr[i] = in_arr[i];
                    // else duplicate the last value
                }
                else if (i > 0)
                {
                    arr[i] = arr[i - 1];
                }
                else
                {
                    throw new System.InvalidOperationException("Error in filling profile array");
                }
            }
            return arr;
        }
    }

    public static class eLaneW
    {
        public const double std = 3.65;
        public const double nar = 2.9;

        public static double Get(eLaneWidthMode input)
        {
            switch (input)
            {
                case eLaneWidthMode.nar:
                    return nar;
                default:
                    return std;
            }
        }
    }
    public static class eLaneTexOffsetW
    {
        public const double std = 0;
        public const double nar = 0.102;

        public static double Get(eLaneWidthMode input)
        {
            switch (input)
            {
                case eLaneWidthMode.nar:
                    return nar;
                default:
                    return std;
            }
        }
    }
    public static class eLanePathW
    {
        public const double std = 3.3;
        public const double nar = 2.7;

        public static double Get(eLaneWidthMode input)
        {
            switch (input)
            {
                case eLaneWidthMode.nar:
                    return nar;
                default:
                    return std;
            }
        }
    }
    public enum eLaneWidthMode
    {
        std = 0,
        nar = 1
    }

    public static class eShoulderW
    {
        public const double non = 0.2;
        public const double nar = 0.7;
        public const double med = 1.3;
        public const double wid = 2.4;

        public static double Get(int input)
        {
            switch (input)
            {
                case 1:
                    return nar;
                case 2:
                    return med;
                case 3:
                    return wid;
                case 4:
                    return eLaneW.std;
                case 5:
                    return eLaneW.nar;
                default:
                    return non;
            }
        }

        public static double Get(eShoulderWidthMode input)
        {
            switch (input)
            {
                case eShoulderWidthMode.nar:
                    return nar;
                case eShoulderWidthMode.med:
                    return med;
                case eShoulderWidthMode.wid:
                    return wid;
                default:
                    return non;
            }
        }
    }
    public enum eShoulderWidthMode
    {
        non = 0,
        nar = 1,
        med = 2,
        wid = 3,
        lan = 4
    }

    public static class ePavW
    {
        public const double exw = 7.00;
        public const double wid = 3.75;
        public const double std = 2.50;
        public const double nar = 1.25;

        public static double Get(ePavWidthMode input)
        {
            switch (input)
            {
                case ePavWidthMode.nar:
                    return nar;
                case ePavWidthMode.wid:
                    return wid;
                case ePavWidthMode.exw:
                    return exw;
                default:
                    return std;
            }
        }
    }
    public static class ePavTexW
    {
        public const double exw = 1.918;
        public const double wid = 1.027;
        public const double std = 0.685;
        public const double nar = 0.342;

        public static double Get(ePavWidthMode input)
        {
            switch (input)
            {
                case ePavWidthMode.nar:
                    return nar;
                case ePavWidthMode.wid:
                    return wid;
                case ePavWidthMode.exw:
                    return exw;
                default:
                    return std;
            }
        }
    }
    public static class ePavPathW
    {
        public const double exw = 6.0;
        public const double wid = 3.0;
        public const double std = 2.0;
        public const double nar = 1.0;

        public static double Get(ePavWidthMode input)
        {
            switch (input)
            {
                case ePavWidthMode.nar:
                    return nar;
                case ePavWidthMode.wid:
                    return wid;
                case ePavWidthMode.exw:
                    return exw;
                default:
                    return std;
            }
        }
    }
    public enum ePavWidthMode
    {
        nar = 0,
        std = 1,
        wid = 2,
        exw = 3
    }

    public class eLaneEnabled
    {
        public bool left = false;
        public bool midLeft = false;
        public bool centreLeft = false;
        public bool centre = false;
        public bool centreRight = false;
        public bool midRight = false;
        public bool right = false;

        public int Count()
        {
            int count = 0;
            count += Convert.ToInt32(left);
            count += Convert.ToInt32(midLeft);
            count += Convert.ToInt32(centreLeft);
            count += Convert.ToInt32(centre);
            count += Convert.ToInt32(centreRight);
            count += Convert.ToInt32(midRight);
            count += Convert.ToInt32(right);
            return count;
        }
    }

    public static class eHLineW
    {
        public const double std = 0.05;
        public const double wid = 0.075;
        public const double vwid = 0.1;
    }

    public class texData
    {
        public string name;
        public int alpha;
        public int index = 0;

        public texData(string in_name, int in_alpha)
        {
            name = in_name;
            alpha = in_alpha;
        }

        public void SetIndex(int in_index)
        {
            index = in_index;
        }

        public (string, int) GetNameAlpha()
        {
            return (name, alpha);
        }

        public int GetIndex()
        {
            return index;
        }
    }
    //public class eTex
    //{
    //    public string asph_norm = "Asphalt Norm";
    //    public string asph_light = "Asphalt Norm";
    //    public string asph_mid = "Asphalt Norm";
    //    public string asph_dark = "Asphalt Norm";
    //    public string verge = "Asphalt Norm";
    //    public string white = "Asphalt Norm";
    //    public string yello = "Asphalt Norm";
    //    public string hatch = "Asphalt Norm";
    //    public string kerbs = "Kerbs";
    //    public string pavmt = "Pavement";
    //    public string terra = "Terrain";
    //}

    public static class eTex
    {
        private const string worn = " Worn";
        //private const string verg = " Verg";

        public const string asph_norm = "Asphalt Norm";
        public const string asph_norm_worn = asph_norm + worn;
        //public const string asph_norm_verg = asph_norm + verg;

        public const string asph_light = "Asphalt Light";
        public const string asph_light_worn = asph_light + worn;
        //public const string asph_light_verg = asph_light + verg;

        public const string asph_mid = "Asphalt Mid";
        public const string asph_mid_worn = asph_mid + worn;
        //public const string asph_mid_verg = asph_mid + verg;

        public const string asph_dark = "Asphalt Dark";
        public const string asph_dark_worn = asph_dark + worn;
        //public const string asph_dark_verg = asph_dark + verg;

        public const string treat_red = "Treatment Red";
        public const string treat_red_worn = treat_red + worn;
        //public const string treat_red_verg = treat_red + verg;

        public const string treat_yel = "Treatment Yellow";
        public const string treat_yel_worn = treat_yel + worn;
        //public const string treat_yel_verg = treat_yel + verg;

        public const string treat_gre = "Treatment Green";
        public const string treat_gre_worn = treat_gre + worn;
        //public const string treat_gre_verg = treat_gre + verg;

        public const string treat_blu = "Treatment Blue";
        public const string treat_blu_worn = treat_blu + worn;
        //public const string treat_blu_verg = treat_blu + verg;

        public const string pav_cobbl = "Paving Cobble";
        public const string pav_cobbl_worn = pav_cobbl + worn;
        //public const string pav_cobbl_verg = pav_cobbl + verg;

        public const string pav_herri = "Paving Herringbone";
        public const string pav_herri_worn = pav_herri + worn;
        //public const string pav_herri_verg = pav_herri + verg;

        public const string pav_slabs = "Paving Slabs Small";
        public const string pav_slabs_worn = pav_slabs + worn;
        //public const string pav_slabs_verg = pav_slabs + verg;

        public const string pav_slabl = "Paving Slabs Large";
        public const string pav_slabl_worn = pav_slabl + worn;
        //public const string pav_slabl_verg = pav_slabl + verg;

        public const string gravel = "Gravel";
        public const string gravel_worn = gravel + worn;
        //public const string gravel_slabl_verg = gravel + verg;

        public const string verge = "Verge";
        public const string white = "White";
        public const string yello = "Yellow";
        public const string hatch = "Hatching";
        public const string kerbs = "Kerbs";
        public const string terra = "Terrain";
    }

    //"Asphalt Norm",
    //    "Asphalt Light",
    //    "Asphalt Mid",
    //    "Asphalt Dark",
    //    "Treatment Red",
    //    "Treatment Yellow",
    //    "Treatment Green",
    //    "Treatment Blue",
    //    "Paving Cobble",
    //    "Paving Herringbone",
    //    "Paving Slabs Small",
    //    "Paving Slabs Large",
    //    "Gravel"

    public enum ePathType
    {
        car = 0,
        ped = 1
    }
    public enum ePathDir
    {
        fwd = 0,
        rev = 1,
        bth = 2
    }
}