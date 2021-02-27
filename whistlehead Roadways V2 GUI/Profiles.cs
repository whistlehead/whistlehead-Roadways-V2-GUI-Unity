using System;
using System.Collections.Generic;

namespace whistlehead_Roadways_V2_GUI
{
    public class Profiles
    {
        public static readonly double centre = 0;

        public static readonly double w_verg = 0.2;

        public static readonly double w_gras = 2;
        public static readonly double w_kerb = 0.15;

        public static readonly double h_lane = 0.1;
        public static readonly double h_line = 0.105;
        public static readonly double h_kerb = 0.25;

        public static readonly double doubleline_offset = 0.2;
        public static readonly double doubleline_offset_wid = 0.3;
        public static readonly double kerb_slope_offset = 0.05;

        public static readonly double rep_surf = 1 / eLaneW.std;
        public static readonly double rep_3m = 0.333;
        public static readonly double rep_4m = 0.278;
        public static readonly double rep_5m = 0.2;
        public static readonly double rep_6m = 0.167;
        public static readonly double rep_9m = 0.111;

        public static readonly double hatch_factor = (double)2 / 6;

        public static readonly List<profData> data_lane_std = new List<profData> { new profData(2, eTex.asph_norm, new double[] { centre, eLaneW.std }, h_lane, new double[] { 1.000 - eLaneTexOffsetW.std, 0.000 + eLaneTexOffsetW.std }, rep_surf) };
        public static readonly List<profData> data_lane_nar = new List<profData> { new profData(2, eTex.asph_norm, new double[] { centre, eLaneW.nar }, h_lane, new double[] { 1.000 - eLaneTexOffsetW.nar, 0.000 + eLaneTexOffsetW.nar }, rep_surf) };
        public static readonly List<List<profData>> data_lane = new List<List<profData>> { data_lane_std, data_lane_nar };

        public static readonly List<profData> data_lane_chevrons_std = new List<profData> { new profData(2, eTex.hatch, new double[] { 0.151, eLaneW.std - 0.151 }, h_line - 0.001, new double[] { (eLaneW.std - 0.302) * hatch_factor, 0.000 }, rep_6m) };
        public static readonly List<profData> data_lane_chevrons_nar = new List<profData> { new profData(2, eTex.hatch, new double[] { 0.151, eLaneW.nar - 0.151 }, h_line - 0.001, new double[] { (eLaneW.nar - 0.302) * hatch_factor, 0.000 }, rep_6m) };
        public static readonly List<List<profData>> data_lane_chevrons = new List<List<profData>> { data_lane_chevrons_std, data_lane_chevrons_nar };

        public static readonly List<profData> data_lane_chevrons_inv_std = new List<profData> { new profData(2, eTex.hatch, new double[] { 0.151, eLaneW.std - 0.151 }, h_line - 0.001, new double[] { 0.000, (eLaneW.std - 0.302) * hatch_factor }, rep_6m) };
        public static readonly List<profData> data_lane_chevrons_inv_nar = new List<profData> { new profData(2, eTex.hatch, new double[] { 0.151, eLaneW.nar - 0.151 }, h_line - 0.001, new double[] { 0.000, (eLaneW.nar - 0.302) * hatch_factor }, rep_6m) };
        public static readonly List<List<profData>> data_lane_chevrons_inv = new List<List<profData>> { data_lane_chevrons_inv_std, data_lane_chevrons_inv_nar };

        public static readonly List<profData> data_lane_chevrons_both_std = new List<profData> { new profData(3, eTex.hatch, new double[] { 0.151, 0.151 + (eLaneW.std / 2), eLaneW.std - 0.151 }, h_line - 0.001, new double[] { ((eLaneW.std / 2) - 0.302) * hatch_factor, 0.000, ((eLaneW.std / 2) - 0.302) * hatch_factor }, rep_6m) };
        public static readonly List<profData> data_lane_chevrons_both_nar = new List<profData> { new profData(3, eTex.hatch, new double[] { 0.151, 0.151 + (eLaneW.nar / 2), eLaneW.nar - 0.151 }, h_line - 0.001, new double[] { ((eLaneW.nar / 2) - 0.302) * hatch_factor, 0.000, ((eLaneW.nar / 2) - 0.302) * hatch_factor }, rep_6m) };
        public static readonly List<List<profData>> data_lane_chevrons_both = new List<List<profData>> { data_lane_chevrons_both_std, data_lane_chevrons_both_std };

        public static readonly List<profData> data_lane_chevrons_both_inv_std = new List<profData> { new profData(3, eTex.hatch, new double[] { 0.151, 0.151 + (eLaneW.std / 2), eLaneW.std - 0.151 }, h_line - 0.001, new double[] { 0.000, ((eLaneW.std / 2) - 0.302) * hatch_factor, 0.000 }, rep_6m) };
        public static readonly List<profData> data_lane_chevrons_both_inv_nar = new List<profData> { new profData(3, eTex.hatch, new double[] { 0.151, 0.151 + (eLaneW.nar / 2), eLaneW.nar - 0.151 }, h_line - 0.001, new double[] { 0.000, ((eLaneW.nar / 2) - 0.302) * hatch_factor, 0.000 }, rep_6m) };
        public static readonly List<List<profData>> data_lane_chevrons_both_inv = new List<List<profData>> { data_lane_chevrons_both_inv_std, data_lane_chevrons_both_inv_nar };

        public static readonly List<profData> data_pav_nar = new List<profData> { new profData(2, eTex.asph_norm, new double[] { centre, ePavW.nar }, h_kerb, new double[] { 0.000, ePavTexW.nar }, rep_surf) };
        public static readonly List<profData> data_pav_std = new List<profData> { new profData(2, eTex.asph_norm, new double[] { centre, ePavW.std }, h_kerb, new double[] { 0.000, ePavTexW.std }, rep_surf) };
        public static readonly List<profData> data_pav_wid = new List<profData> { new profData(2, eTex.asph_norm, new double[] { centre, ePavW.wid }, h_kerb, new double[] { 0.000, ePavTexW.wid }, rep_surf) };
        public static readonly List<profData> data_pav_exw = new List<profData> { new profData(2, eTex.asph_norm, new double[] { centre, ePavW.exw }, h_kerb, new double[] { 0.000, ePavTexW.exw }, rep_surf) };
        public static readonly List<List<profData>> data_pav = new List<List<profData>> { data_pav_nar, data_pav_std, data_pav_wid, data_pav_exw };

        public static readonly List<profData> data_kerb_single = new List<profData> { new profData(2, eTex.kerbs, new double[] { centre, centre }, new double[] { h_lane, h_kerb }, new double[] { 1.000, 0.500 }, rep_5m), new profData(2, eTex.kerbs, new double[] { centre, w_kerb }, new double[] { h_kerb, h_kerb }, new double[] { 0.500, 0.000 }, rep_5m) };
        public static readonly List<profData> data_kerb_slope_single = new List<profData> { new profData(2, eTex.kerbs, new double[] { centre, centre }, new double[] { h_lane, h_kerb - kerb_slope_offset }, new double[] { 1.000, 0.500 }, rep_5m), new profData(2, eTex.kerbs, new double[] { centre, kerb_slope_offset }, new double[] { h_kerb - kerb_slope_offset, h_kerb }, new double[] { 0.500, 0.300 }, rep_5m), new profData(2, "Kerbs", new double[] { kerb_slope_offset, w_kerb }, new double[] { h_kerb, h_kerb }, new double[] { 0.300, 0.000 }, rep_5m) };
        public static readonly List<List<profData>> data_kerbs_single = new List<List<profData>> { data_kerb_single, data_kerb_slope_single };

        public static readonly List<profData> data_kerb_double = new List<profData> { new profData(2, eTex.kerbs, new double[] { centre, centre }, new double[] { h_lane, h_kerb }, new double[] { 1.000, 0.500 }, rep_5m), new profData(2, eTex.kerbs, new double[] { centre, w_kerb }, new double[] { h_kerb, h_kerb }, new double[] { 0.500, 0.000 }, rep_5m), new profData(2, "Kerbs", new double[] { w_kerb, w_kerb }, new double[] { h_kerb, h_lane }, new double[] { 0.500, 1.000 }, rep_5m) };
        public static readonly List<profData> data_kerb_slope_double = new List<profData> { new profData(2, eTex.kerbs, new double[] { centre, centre }, new double[] { h_lane, h_kerb - kerb_slope_offset }, new double[] { 1.000, 0.500 }, rep_5m), new profData(2, eTex.kerbs, new double[] { centre, kerb_slope_offset }, new double[] { h_kerb - kerb_slope_offset, h_kerb }, new double[] { 0.500, 0.300 }, rep_5m), new profData(2, "Kerbs", new double[] { kerb_slope_offset, w_kerb }, new double[] { h_kerb, h_kerb }, new double[] { 0.300, 0.000 }, rep_5m), new profData(2, "Kerbs", new double[] { w_kerb, w_kerb }, new double[] { h_kerb, h_lane }, new double[] { 0.500, 1.000 }, rep_5m) };
        public static readonly List<List<profData>> data_kerbs_double = new List<List<profData>> { data_kerb_double, data_kerb_slope_double };

        public static readonly List<profData> data_grass_slope = new List<profData> { new profData(2, eTex.terra, new double[] { centre, w_gras }, new double[] { h_kerb, 0 }, new double[] { 0.000, 1.000 }, 1 / w_gras) };
        public static readonly List<profData> data_grass_pavof = new List<profData> { new profData(2, eTex.terra, new double[] { centre, w_gras }, new double[] { h_kerb, h_kerb }, new double[] { 0.000, 1.000 }, 1 / w_gras) };

        public static readonly List<profData> data_edge_non = new List<profData> { new profData(2, eTex.asph_norm, new double[] { centre, eShoulderW.non }, h_lane, new double[] { 0.000, 0.055 }, rep_surf) };
        public static readonly List<profData> data_edge_nar = new List<profData> { new profData(2, eTex.asph_norm, new double[] { centre, eShoulderW.nar }, h_lane, new double[] { 0.000, 0.192 }, rep_surf) };
        public static readonly List<profData> data_edge_med = new List<profData> { new profData(2, eTex.asph_norm, new double[] { centre, eShoulderW.med }, h_lane, new double[] { 0.000, 0.356 }, rep_surf) };
        public static readonly List<profData> data_edge_wid = new List<profData> { new profData(2, eTex.asph_norm, new double[] { centre, eShoulderW.wid }, h_lane, new double[] { 0.000, 0.657 }, rep_surf) };
        public static readonly List<profData> data_edge_lan_std = new List<profData> { new profData(2, eTex.asph_norm, new double[] { centre, eLaneW.std }, h_lane, new double[] { 1.000 - eLaneTexOffsetW.std, 0.000 + eLaneTexOffsetW.std }, rep_surf) };
        public static readonly List<profData> data_edge_lan_nar = new List<profData> { new profData(2, eTex.asph_norm, new double[] { centre, eLaneW.nar }, h_lane, new double[] { 1.000 - eLaneTexOffsetW.nar, 0.000 + eLaneTexOffsetW.nar }, rep_surf) };
        public static readonly List<List<profData>> data_edge = new List<List<profData>> { data_edge_non, data_edge_nar, data_edge_med, data_edge_wid, data_edge_lan_std, data_edge_lan_nar };

        //public static readonly List<profData> data_verge_nar = new List<profData> { new profData(2, eTex.asph_norm_verg, new double[] { centre, eShoulderW.nar + w_verg }, h_lane, new double[] { 0.120, 0.010 }, rep_5m) };
        //public static readonly List<profData> data_verge_med = new List<profData> { new profData(2, eTex.asph_norm_verg, new double[] { centre, eShoulderW.med + w_verg }, h_lane, new double[] { 0.394, 0.010 }, rep_5m) };
        //public static readonly List<profData> data_verge_wid = new List<profData> { new profData(2, eTex.asph_norm_verg, new double[] { centre, eShoulderW.wid + w_verg }, h_lane, new double[] { 0.667, 0.010 }, rep_5m) };
        //public static readonly List<List<profData>> data_verge = new List<List<profData>> { data_verge_nar, data_verge_med, data_verge_wid };

        public static readonly List<profData> data_verge = new List<profData> { new profData(2, eTex.verge, new double[] { centre, w_verg }, h_lane, new double[] { 0.850, 0.100 }, rep_5m) };

        //public static readonly List<profData> data_edgeline_norm = new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.281, 0.313 }, rep_6m) };
        //public static readonly List<profData> data_edgeline_junc = new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.688, 0.719 }, rep_4m) };
        //public static readonly List<profData> data_edgeline_singleyellow = new List<profData> { new profData(2, eTex.yello, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.500, 0.625 }, rep_4m) };
        //public static readonly List<profData> data_edgeline_doubleyellow = new List<profData> { new profData(2, eTex.yello, new double[] { -eHLineW.std - doubleline_offset, eHLineW.std - doubleline_offset }, h_line, new double[] { 0.500, 0.625 }, rep_6m), new profData(2, eTex.yello, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.500, 0.625 }, rep_6m) };
        public static readonly lineMetaData data_edgeline_10_norm = new lineMetaData("10cm Normal (RH: EdgeLine_Norm_100)", "E10No", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.281, 0.313 }, rep_6m) });
        public static readonly lineMetaData data_edgeline_10_junc = new lineMetaData("10cm Junction (RH: EdgeLine_Junc_100)", "E10Ju", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.688, 0.719 }, rep_4m) });

        public static readonly lineMetaData data_edgeline_15_norm = new lineMetaData("15cm Normal (RH: EdgeLine_Norm_150)", "E15No", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.wid, eHLineW.wid }, h_line, new double[] { 0.281, 0.313 }, rep_6m) });
        public static readonly lineMetaData data_edgeline_15_junc = new lineMetaData("15cm Junction (RH: EdgeLine_Junc_150)", "E15Ju", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.wid, eHLineW.wid }, h_line, new double[] { 0.688, 0.719 }, rep_4m) });
        
        public static readonly lineMetaData data_edgeline_10_singleyellow = new lineMetaData("10cm Single yellow (RH: Singleyellow_100)", "E10Sy", new List<profData> { new profData(2, eTex.yello, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.500, 0.625 }, rep_4m) });
        public static readonly lineMetaData data_edgeline_10_doubleyellow = new lineMetaData("10cm Double yellow (RH: Doubleyellow_100)", "E10Dy", new List<profData> { new profData(2, eTex.yello, new double[] { -eHLineW.std - doubleline_offset, eHLineW.std - doubleline_offset }, h_line, new double[] { 0.500, 0.625 }, rep_6m), new profData(2, eTex.yello, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.500, 0.625 }, rep_6m) });

        //public static readonly List<profData> data_centreline_norm_std = new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.406, 0.438 }, rep_6m) };
        //public static readonly List<profData> data_centreline_warn_std = new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.344, 0.375 }, rep_6m) };
        //public static readonly List<profData> data_centreline_norm2_std = new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.406, 0.438 }, rep_9m) };
        //public static readonly List<profData> data_centreline_warn2_std = new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.344, 0.375 }, rep_9m) };
        //public static readonly List<profData> data_centreline_norm_wid = new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.wid, eHLineW.wid }, h_line, new double[] { 0.406, 0.438 }, rep_6m) };
        //public static readonly List<profData> data_centreline_warn_wid = new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.wid, eHLineW.wid }, h_line, new double[] { 0.344, 0.375 }, rep_6m) };
        //public static readonly List<profData> data_chevrons_1000 = new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std - 0.450, eHLineW.std - 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.std + 0.450, eHLineW.std + 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(2, eTex.hatch, new double[] { -0.300, 0.300 }, h_line, new double[] { 0.200, 0.000 }, rep_6m) };
        //public static readonly List<profData> data_chevrons_inv_1000 = new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std - 0.450, eHLineW.std - 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.std + 0.450, eHLineW.std + 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(2, eTex.hatch, new double[] { -0.300, 0.300 }, h_line, new double[] { 0.000, 0.200 }, rep_6m) };
        //public static readonly List<profData> data_chevrons_both_1000 = new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std - 0.450, eHLineW.std - 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.std + 0.450, eHLineW.std + 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(3, eTex.hatch, new double[] { -0.300, 0.000, 0.300 }, h_line, new double[] { 0.100, 0.000, 0.100 }, rep_6m) };
        //public static readonly List<profData> data_chevrons_both_inv_1000 = new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std - 0.450, eHLineW.std - 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.std + 0.450, eHLineW.std + 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(3, eTex.hatch, new double[] { -0.300, 0.000, 0.300 }, h_line, new double[] { 0.000, 0.100, 0.000 }, rep_6m) };
        public static readonly lineMetaData data_centreline_10_solid = new lineMetaData("10cm Solid (RH: EdgeLine_Norm_100)", "C10So", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.281, 0.313 }, rep_6m) });
        public static readonly lineMetaData data_centreline_10_norm = new lineMetaData("10cm Normal (RH: CentreLine_Norm_100)", "C10No", new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.406, 0.438 }, rep_6m));
        public static readonly lineMetaData data_centreline_10_norm2 = new lineMetaData("10cm Normal 2 (RH: CentreLine_Norm2_100)", "C10No2", new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.406, 0.438 }, rep_9m));
        public static readonly lineMetaData data_centreline_10_lane = new lineMetaData("10cm Lane (RH: CentreLine_Lane_100)", "C10No", new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.469, 0.500 }, rep_6m));
        public static readonly lineMetaData data_centreline_10_lane2 = new lineMetaData("10cm Lane 2 (RH: CentreLine_Lane2_100)", "C10No2", new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.469, 0.500 }, rep_9m));
        public static readonly lineMetaData data_centreline_10_warn = new lineMetaData("10cm Warning (RH: CentreLine_Warn_100)", "C10Wa", new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.344, 0.375 }, rep_6m));
        public static readonly lineMetaData data_centreline_10_warn2 = new lineMetaData("10cm Warning 2 (RH: CentreLine_Warn2_100)", "C10Wa2", new profData(2, eTex.white, new double[] { -eHLineW.std, eHLineW.std }, h_line, new double[] { 0.344, 0.375 }, rep_9m));
        public static readonly lineMetaData data_centreline_10_double = new lineMetaData("10cm Double Solid (RH: CentreLine_Double_Solid_100)", "E10Db", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std - doubleline_offset / 2, eHLineW.std - doubleline_offset / 2}, h_line, new double[] { 0.281, 0.313 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.std + doubleline_offset / 2, eHLineW.std + doubleline_offset / 2 }, h_line, new double[] { 0.281, 0.313 }, rep_6m) });
        public static readonly lineMetaData data_centreline_10_double_dash_left = new lineMetaData("10cm Double Dash Left (RH: CentreLine_Double_Dash_100)", "E10DdL", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std - doubleline_offset / 2, eHLineW.std - doubleline_offset / 2 }, h_line, new double[] { 0.469, 0.500 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.std + doubleline_offset / 2, eHLineW.std + doubleline_offset / 2 }, h_line, new double[] { 0.281, 0.313 }, rep_6m) });
        public static readonly lineMetaData data_centreline_10_double_dash_right = new lineMetaData("10cm Double Dash Right (RH: CentreLine_Double_Dash_100)", "E10DdR", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std - doubleline_offset / 2, eHLineW.std - doubleline_offset / 2 }, h_line, new double[] { 0.281, 0.313 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.std + doubleline_offset / 2, eHLineW.std + doubleline_offset / 2 }, h_line, new double[] { 0.469, 0.500 }, rep_6m) });

        public static readonly lineMetaData data_centreline_15_solid = new lineMetaData("15cm Solid (RH: EdgeLine_Norm_150)", "C15So", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.wid, eHLineW.wid }, h_line, new double[] { 0.281, 0.313 }, rep_6m) });
        public static readonly lineMetaData data_centreline_15_norm = new lineMetaData("15cm Normal (RH: CentreLine_Norm_150)", "C15No", new profData(2, eTex.white, new double[] { -eHLineW.wid, eHLineW.wid }, h_line, new double[] { 0.406, 0.438 }, rep_6m));
        public static readonly lineMetaData data_centreline_15_norm2 = new lineMetaData("15cm Normal 2 (RH: CentreLine_Norm2_150)", "C15No2", new profData(2, eTex.white, new double[] { -eHLineW.wid, eHLineW.wid }, h_line, new double[] { 0.406, 0.438 }, rep_9m));
        public static readonly lineMetaData data_centreline_15_lane = new lineMetaData("15cm Lane (RH: CentreLine_Lane_150)", "C15No", new profData(2, eTex.white, new double[] { -eHLineW.wid, eHLineW.wid }, h_line, new double[] { 0.469, 0.500 }, rep_6m));
        public static readonly lineMetaData data_centreline_15_lane2 = new lineMetaData("15cm Lane 2 (RH: CentreLine_Lane2_150)", "C15No2", new profData(2, eTex.white, new double[] { -eHLineW.wid, eHLineW.wid }, h_line, new double[] { 0.469, 0.500 }, rep_9m));
        public static readonly lineMetaData data_centreline_15_warn = new lineMetaData("15cm Warning (RH: CentreLine_Warn_150)", "C15Wa", new profData(2, eTex.white, new double[] { -eHLineW.wid, eHLineW.wid }, h_line, new double[] { 0.344, 0.375 }, rep_6m));
        public static readonly lineMetaData data_centreline_15_warn2 = new lineMetaData("15cm Warning 2 (RH: CentreLine_Warn2_150)", "C15Wa2", new profData(2, eTex.white, new double[] { -eHLineW.wid, eHLineW.wid }, h_line, new double[] { 0.344, 0.375 }, rep_9m));
        public static readonly lineMetaData data_centreline_15_double = new lineMetaData("15cm Double Solid (RH: CentreLine_Double_Solid_150)", "E15Db", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.wid - doubleline_offset_wid / 2, eHLineW.wid - doubleline_offset_wid / 2 }, h_line, new double[] { 0.281, 0.313 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.wid + doubleline_offset_wid / 2, eHLineW.wid + doubleline_offset_wid / 2 }, h_line, new double[] { 0.281, 0.313 }, rep_6m) });
        public static readonly lineMetaData data_centreline_15_double_dash_left = new lineMetaData("15cm Double Dash Left (RH: CentreLine_Double_Dash_150)", "E15DdL", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.wid - doubleline_offset_wid / 2, eHLineW.wid - doubleline_offset_wid / 2 }, h_line, new double[] { 0.469, 0.500 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.wid + doubleline_offset_wid / 2, eHLineW.wid + doubleline_offset_wid / 2 }, h_line, new double[] { 0.281, 0.313 }, rep_6m) });
        public static readonly lineMetaData data_centreline_15_double_dash_right = new lineMetaData("15cm Double Dash Right (RH: CentreLine_Double_Dash_150)", "E15DdR", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.wid - doubleline_offset_wid / 2, eHLineW.wid - doubleline_offset_wid / 2 }, h_line, new double[] { 0.281, 0.313 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.wid + doubleline_offset_wid / 2, eHLineW.wid + doubleline_offset_wid / 2 }, h_line, new double[] { 0.469, 0.500 }, rep_6m) });

        public static readonly lineMetaData data_chevrons_1000 = new lineMetaData("Chevrons (1m) (RH: Chevrons_1000)", "CCv", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std - 0.450, eHLineW.std - 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.std + 0.450, eHLineW.std + 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(2, eTex.hatch, new double[] { -0.300, 0.300 }, h_line, new double[] { 0.200, 0.000 }, rep_6m) });
        public static readonly lineMetaData data_chevrons_inv_1000 = new lineMetaData("Chevrons reversed (1m) (RH: Chevrons_inv_1000)", "CCvR", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std - 0.450, eHLineW.std - 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.std + 0.450, eHLineW.std + 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(2, eTex.hatch, new double[] { -0.300, 0.300 }, h_line, new double[] { 0.000, 0.200 }, rep_6m) });
        public static readonly lineMetaData data_chevrons_both_1000 = new lineMetaData("Chevrons double (1m) (RH: Chevrons_both_1000)", "CCvB", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std - 0.450, eHLineW.std - 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.std + 0.450, eHLineW.std + 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(3, eTex.hatch, new double[] { -0.300, 0.000, 0.300 }, h_line, new double[] { 0.100, 0.000, 0.100 }, rep_6m) });
        public static readonly lineMetaData data_chevrons_both_inv_1000 = new lineMetaData("Chevrons double reversed (1m) (RH: Chevrons_both_1000)", "CCvBR", new List<profData> { new profData(2, eTex.white, new double[] { -eHLineW.std - 0.450, eHLineW.std - 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(2, eTex.white, new double[] { -eHLineW.std + 0.450, eHLineW.std + 0.450 }, h_line, new double[] { 0.344, 0.375 }, rep_6m), new profData(3, eTex.hatch, new double[] { -0.300, 0.000, 0.300 }, h_line, new double[] { 0.000, 0.100, 0.000 }, rep_6m) });

        public static readonly List<lineMetaData> data_edgeline = new List<lineMetaData> {
            data_edgeline_10_norm,
            data_edgeline_10_junc,
            data_edgeline_15_norm,
            data_edgeline_15_junc,
            data_edgeline_10_singleyellow,
            data_edgeline_10_doubleyellow
        };
        public static readonly List<lineMetaData> data_centreline = new List<lineMetaData> {
            data_centreline_10_solid,
            data_centreline_10_norm,
            data_centreline_10_norm2,
            data_centreline_10_lane,
            data_centreline_10_lane2,
            data_centreline_10_warn,
            data_centreline_10_warn2,
            data_centreline_10_double,
            data_centreline_10_double_dash_left,
            data_centreline_10_double_dash_right,

            data_centreline_15_solid,
            data_centreline_15_norm,
            data_centreline_15_norm2,
            data_centreline_15_lane,
            data_centreline_15_lane2,
            data_centreline_15_warn,
            data_centreline_15_warn2,
            data_centreline_15_double,
            data_centreline_15_double_dash_left,
            data_centreline_15_double_dash_right,

            data_chevrons_1000,
            data_chevrons_inv_1000,
            data_chevrons_both_1000,
            data_chevrons_both_inv_1000
        };
        // data_edgecentreline must be identical to data_edgeline up to the length of data_edgeline
        public static readonly List<lineMetaData> data_edgecentreline = new List<lineMetaData>
        {
            data_edgeline_10_norm,
            data_edgeline_10_junc,
            data_edgeline_15_norm,
            data_edgeline_15_junc,
            data_edgeline_10_singleyellow,
            data_edgeline_10_doubleyellow,

            data_centreline_10_solid,
            data_centreline_10_norm,
            data_centreline_10_norm2,
            data_centreline_10_lane,
            data_centreline_10_lane2,
            data_centreline_10_warn,
            data_centreline_10_warn2,
            data_centreline_10_double,
            data_centreline_10_double_dash_left,
            data_centreline_10_double_dash_right,

            data_centreline_15_solid,
            data_centreline_15_norm,
            data_centreline_15_norm2,
            data_centreline_15_lane,
            data_centreline_15_lane2,
            data_centreline_15_warn,
            data_centreline_15_warn2,
            data_centreline_15_double,
            data_centreline_15_double_dash_left,
            data_centreline_15_double_dash_right,

            data_chevrons_1000,
            data_chevrons_inv_1000,
            data_chevrons_both_1000,
            data_chevrons_both_inv_1000
        };
    }
}