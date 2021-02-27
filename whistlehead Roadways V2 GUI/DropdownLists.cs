using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whistlehead_Roadways_V2_GUI
{
    class DropdownLists
    {
        public static readonly string[] carriageways = new string[]
        {
        "Single carriageway",
        "Dual carriageway"
        };

        public static readonly string[] SC_lanes = new string[]
        {
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        };

        public static readonly string[] DC_lanes = new string[]
        {
        "1",
        "2",
        "3",
        "4"
        };

        public static readonly string[] laneWidth = new string[]
        {
        "Standard (3.65m / 12ft)",
        "Narrow (2.9m / 9ft6in)"
        };

        public static readonly string[] lane = new string[]
        {
            eTex.asph_norm,
            eTex.asph_light,
            eTex.asph_mid,
            eTex.asph_dark,
            eTex.treat_red,
            eTex.treat_yel,
            eTex.treat_gre,
            eTex.treat_blu,
            eTex.pav_cobbl,
            eTex.pav_herri,
            eTex.pav_slabs,
            eTex.pav_slabl,
            eTex.gravel
        };

        public static readonly string[] shoulder = new string[]
        {
            "Get from adjacent lane",
            eTex.asph_norm,
            eTex.asph_light,
            eTex.asph_mid,
            eTex.asph_dark,
            eTex.treat_red,
            eTex.treat_yel,
            eTex.treat_gre,
            eTex.treat_blu,
            eTex.pav_cobbl,
            eTex.pav_herri,
            eTex.pav_slabs,
            eTex.pav_slabl,
            eTex.gravel
        };

        public static readonly string[] pavement = new string[]
        {
            eTex.asph_norm,
            eTex.asph_light,
            eTex.asph_mid,
            eTex.asph_dark,
            eTex.treat_red,
            eTex.treat_yel,
            eTex.treat_gre,
            eTex.treat_blu,
            eTex.pav_cobbl,
            eTex.pav_herri,
            eTex.pav_slabs,
            eTex.pav_slabl,
            eTex.gravel
        };

        public static readonly string[] edge = new string[]
        {
        "None",
        "Soft edge",
        "Hard edge",
        "Kerb",
        "Kerb with grass slope",
        "Pavement",
        "Pavement with grass slope",
        "Offset pavement",
        "Offset pavement with grass slope",
        };

        public static readonly string[] shoulderMode = new string[]
        {
        "None (0.2m)",
        "Narrow (0.7m)",
        "Medium (1.3m)",
        "Wide (2.4m)",
        "Lane width"
        };

        public static readonly string[] pavementWidth = new string[]
        {
        "Narrow (1.25m)",
        "Standard (2.5m)",
        "Wide (3.75m)",
        "Extended (7m)"
        };

        public static readonly string[] laneMode = new string[]
        {
        "Norm",
        "Worn",
        "Chevrons",
        "Chevrons reversed",
        "Double Chevrons",
        "Double Chevrons reversed"
        };

        public static readonly string[] kerb = new string[]
        {
        "Norm",
        "Sloped"
        };

        //public static readonly string[] edgeMarking = new string[]
        //{
        //"None",
        //"Normal (RH: EdgeLine_Norm_100)",
        //"Junction (RH: EdgeLine_Junc_100)",
        //"Single yellow (RH: Singleyellow_100)",
        //"Double yellow (RH: Doubleyellow_100)",
        //"Lane Normal (RH: CentreLine_Norm_100)",
        //"Lane Normal 2 (RH: CentreLine_Norm2_100)",
        //"Lane Warning (RH: CentreLine_Warn_100)",
        //"Lane Warning 2 (RH: CentreLine_Warn2_100)",
        //};

        //public static readonly string[] laneMarking = new string[]
        //{
        //"None",
        //"Solid (RH: EdgeLine_Norm_100)",
        //"Normal (RH: CentreLine_Norm_100)",
        //"Normal 2 (RH: CentreLine_Norm2_100)",
        //"Warning (RH: CentreLine_Warn_100)",
        //"Warning 2 (RH: CentreLine_Warn2_100)",
        //"Chevrons (1m) (RH: Chevrons_1000)",
        //"Chevrons reversed (1m) (RH: Chevrons_inv_1000)",
        //"Double chevrons (1m) (RH: Chevrons_both_1000)",
        //"Double chevrons reversed (1m) (RH: Chevrons_both_1000)"
        //};

        public static readonly string[] ai = new string[]
        {
        "None",
        "Forward",
        "Reverse",
        "Both"
        };
    }
}
