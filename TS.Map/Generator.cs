using System;
using System.Collections.Generic;
using System.Linq;

namespace TS.Map
{
    public class Generator
    {
        public static Dictionary<string, List<string>> dicChunks;
        public static Dictionary<string, List<string>> dicChunksOpposite;
        public static List<Chunk> listChunks;

        //first
        //0,0_28,16@28,16_51,20@51,20_68,20@68,20_112,19@112,19_117,29@117,29_138,41@138,41_160,35@160,35_184,45@184,45_201,45@201,45_222,49@222,49_240,54@240,54_258,64@258,64_277,69@277,69_297,77@297,77_327,83@327,83_353,94@353,94_396,101@396,101_428,108@428,108_448,114@448,114_509,118@509,118_545,131@545,131_611,145@611,145_648,149@648,149_672,152@672,152_691,167@691,167_731,179@731,179_756,187@756,187_799,193@799,193_823,213@823,213_871,227@871,227_895,223@895,223_922,227@922,227_933,235@933,235_955,240@955,240_973,248@973,248_995,251@995,251_997,253@448,114_453,79@453,79_470,64@470,64_485,49@485,49_497,34@497,34_513,20@513,20_534,8@534,8_547,1@448,114_430,155@430,155_421,186@421,186_412,199@412,199_393,227@393,227_374,260@374,260_375,272@375,272_356,296@356,296_341,312@341,312_334,331@334,331_325,343@325,343_317,359@317,359_310,375@310,375_299,387@299,387_298,399@298,399_280,425@280,425_273,445@273,445_269,472@269,472_252,502@252,502_246,520@246,520_236,540@236,540_233,550@233,550_221,570@221,570_220,581@220,581_204,608@204,608_198,627@198,627_191,647@191,647_185,662@185,662_178,678@178,678_176,694@176,694_174,699@246,520_215,440@215,440_156,417@156,417_115,398@115,398_59,391@59,391_18,386@18,386_3,383@246,520_318,535@318,535_346,541@346,541_358,546@358,546_377,547@377,547_398,554@398,554_438,562@438,562_471,563@471,563_510,565@510,565_544,573@544,573_585,581@585,581_626,587@626,587_668,594@668,594_731,612@731,612_766,630@766,630_812,644@812,644_859,653@859,653_900,670@900,670_926,686@926,686_955,691@955,691_975,697@691,167_781,82@781,82_801,76@801,76_816,66@816,66_832,54@832,54_856,40@856,40_873,40@873,40_902,23@902,23_922,16@922,16_942,11@942,11_961,3@961,3_978,2@438,562_392,471@392,471_429,456@429,456_461,452@461,452_478,457@478,457_497,467@497,467_516,461@516,461_543,464@543,464_589,459@589,459_637,453@637,453_661,441@661,441_718,444@718,444_738,456@738,456_772,476@772,476_815,476@815,476_872,476@872,476_893,489@893,489_928,493@928,493_967,497@967,497_991,510@991,510_998,515@374,260_356,225@356,225_337,225@337,225_309,233@309,233_272,230@272,230_237,206@237,206_214,200@214,200_192,207@192,207_169,216@169,216_155,208@155,208_140,203@140,203_127,202@127,202_116,202@116,202_107,208@107,208_92,208@92,208_77,203@77,203_57,198@57,198_42,198@42,198_32,207@32,207_18,208@18,208_9,202@9,202_2,194@374,260_415,275@415,275_426,276@426,276_431,278@431,278_440,280@440,280_454,285@454,285_463,283@463,283_476,284@476,284_479,289@479,289_498,295@498,295_513,295@513,295_544,304@544,304_553,313@553,313_589,321@589,321_609,320@609,320_626,319@626,319_655,319@655,319_673,306@673,306_698,303@698,303_748,309@748,309_759,313@759,313_773,328@773,328_797,332@797,332_822,332@822,332_829,343@829,343_854,353@854,353_870,367@870,367_875,376@875,376_889,389@889,389_902,393@902,393_916,398@916,398_931,405@931,405_944,416@944,416_961,420@961,420_972,423@972,423_981,430@981,430_990,435@990,435_997,438@626,587_742,635@742,635_733,644@733,644_725,652@725,652_702,649@702,649_665,645@665,645_650,644@650,644_638,645@638,645_630,637@630,637_628,625@628,625_617,618@617,618_602,614@602,614_590,615@590,615_584,627@584,627_582,639@582,639_581,650@581,650_581,671@581,671_568,682@568,682_553,688@553,688_536,694@536,694_528,700

        //second
        //0,0_69,43@69,43_150,59@150,59_231,73@231,73_325,94@325,94_409,113@409,113_524,133@524,133_662,149@662,149_797,156@797,156_902,157@902,157_998,153@524,133_551,236@551,236_569,332@569,332_549,428@549,428_531,486@531,486_509,553@509,553_490,640@490,640_465,695@524,133_618,85@618,85_712,73@712,73_795,45@795,45_885,23@885,23_972,3@569,332_454,314@454,314_340,291@340,291_183,257@183,257_85,266@85,266_1,280@569,332_685,351@685,351_790,371@790,371_878,400@878,400_950,429@950,429_998,497@340,291_330,380@330,380_286,467@286,467_232,544@232,544_139,594@139,594_42,638@42,638_3,668@549,428_670,518@670,518_778,571@778,571_864,627@864,627_901,697@531,486_412,538@412,538_342,606@342,606_262,650@262,650_180,697@409,113_336,180@336,180_235,198@235,198_96,182@96,182_40,130@40,130_4,58
        
        public static bool IsOneway = true;
        public static string MapChunks;
        public static string MapChunks1 = "0,200_60,161@60,161_153,162@153,162_240,133@240,133_326,110@326,110_425,132@425,132_516,160@516,160_600,201@600,201_684,230@684,230_767,260@767,260_867,281@867,281_944,320@944,320_1000,387@600,201_687,144@687,144_760,98@760,98_851,81@851,81_925,63@925,63_999,25@600,201_530,249@530,249_463,292@463,292_378,341@378,341_310,395@310,395_246,461@246,461_168,526@168,526_88,569@88,569_35,622@35,622_0,683@310,395_388,434@388,434_491,455@491,455_592,467@592,467_687,507@687,507_777,560@777,560_878,583@878,583_954,584@954,584_1000,585@687,507_670,592@670,592_615,654@615,654_535,699@326,110_398,48@398,48_480,0";
        public static string MapChunks2 = "0,0_52,42@52,42_111,98@111,98_179,131@179,131_246,155@246,155_316,196@316,196_384,242@384,242_475,276@475,276_562,289@562,289_653,293@653,293_745,301@745,301_836,323@836,323_914,364@914,364_1000,416@246,155_305,105@305,105_382,69@382,69_470,42@470,42_550,0@246,155_193,216@193,216_160,286@160,286_154,356@154,356_150,420@150,420_152,494@152,494_136,570@136,570_106,632@106,632_65,700@150,420_233,435@233,435_311,463@311,463_377,519@377,519_430,589@430,589_471,656@471,656_540,699@562,289_525,364@525,364_489,439@489,439_456,513@456,513_430,589@430,589_353,650@353,650_292,700@836,323_835,410@835,410_869,484@869,484_899,559@899,559_879,643@879,643_825,699@470,42_560,76@560,76_638,119@638,119_728,137@728,137_832,127@832,127_917,97@917,97_1000,58@832,127_911,176@911,176_1000,226@160,286_69,272@69,272_0,292";
        public static string MapChunks3 = "0,666_61,581@61,581_142,524@142,524_209,453@209,453_236,359@236,359_253,266@253,266_305,187@305,187_368,126@368,126_440,72@440,72_515,34@515,34_605,0@0,444_72,429@72,429_150,396@150,396_236,359@236,359_330,353@330,353_425,334@425,334_512,283@512,283_592,236@592,236_691,214@691,214_794,197@794,197_880,136@880,136_936,76@936,76_998,0@450,700_380,621@380,621_379,548@379,548_348,463@348,463_274,410@274,410_236,359@236,359_169,279@169,279_156,184@156,184_104,106@104,106_43,42@43,42_0,0@250,0_222,69@222,69_104,106@1000,674_924,597@924,597_837,567@837,567_739,524@739,524_683,438@683,438_636,329@636,329_592,236@592,236_502,136@502,136_440,72@440,72_380,0@1000,200_903,186@903,186_794,197@794,197_755,284@755,284_766,375@766,375_776,450@776,450_739,524@739,524_651,576@651,576_549,602@549,602_447,593@447,593_379,548@379,548_265,565@265,565_180,654@180,654_89,700@549,602_618,649@618,649_731,663@731,663_835,674@835,674_919,700";

        public static int SelectedMap;

        #region Public Functions
        /// <summary>
        /// Return chunks Array - around 200 chunks
        /// </summary>
        /// <returns></returns>
        public static List<Chunk> GetMap(int MapID)
        {
            SelectedMap = MapID;
            switch (SelectedMap)
            {
                case 0:
                    MapChunks = MapChunks1;
                    break;
                case 1:
                    MapChunks = MapChunks2;
                    break;
                case 2:
                    MapChunks = MapChunks3;
                    break;
            }

            //Singleton Pattern
            //if (listChunks != null)
            //    return listChunks;
            
            listChunks = new List<Chunk>();

            string[] strChunks = MapChunks.Split('@');
            int i=0;//for ID

            int SpeedLimit = 40;
            string BeforeEndPointer = "";

            foreach (string strChunkPointer in strChunks)
            {
                Chunk c=new Chunk();
                
                //1. ID
                c.ID = i++;

                //2. Start Point
                c.X1 = Convert.ToInt32(strChunkPointer.Split('_')[0].Split(',')[0]);
                c.Y1 = Convert.ToInt32(strChunkPointer.Split('_')[0].Split(',')[1]);

                //3. End Point
                c.X2 = Convert.ToInt32(strChunkPointer.Split('_')[1].Split(',')[0]);
                c.Y2 = Convert.ToInt32(strChunkPointer.Split('_')[1].Split(',')[1]);


                //4. New Speed
                if (BeforeEndPointer != c.X1.ToString()+","+c.Y1.ToString())
                {
                    SpeedLimit += 10;
                    if (SpeedLimit == 80)
                        SpeedLimit = 40;
                }

                BeforeEndPointer = c.X2.ToString() + "," + c.Y2.ToString();
                
                //5. Speed Limitation
                c.SpeedLimit = SpeedLimit;
                listChunks.Add(c);
            }

            return listChunks;
        }

        /// <summary>
        /// Return Intersection Array - 6 Intersection
        /// </summary>
        /// <returns></returns>
        public static List<Intersection> GetIntersection(int MapID)
        {
            List<Chunk> listChunks = GetMap(MapID);

            Dictionary<string, List<string>> dicPointers = new Dictionary<string, List<string>>();

            foreach (Chunk cnk in listChunks)
            {
                // Start Pointer
                string strPointer = cnk.X1.ToString() + "," + cnk.Y1.ToString();
                string strPointerEnd = cnk.X2.ToString() + "," + cnk.Y2.ToString();

                if (dicPointers.ContainsKey(strPointer))
                    dicPointers[strPointer].Add(strPointerEnd);
                else
                {
                    List<string> listPointer = new List<string>();
                    listPointer.Add(strPointerEnd);
                    dicPointers.Add(strPointer, listPointer);
                }
                // End Pointer
                strPointer = cnk.X2.ToString() + "," + cnk.Y2.ToString();
                strPointerEnd = cnk.X1.ToString() + "," + cnk.Y1.ToString();
                if (dicPointers.ContainsKey(strPointer))
                {
                    dicPointers[strPointer].Add(strPointerEnd);
                }
                else
                {
                    List<string> listPointer = new List<string>();
                    listPointer.Add(strPointerEnd);
                    dicPointers.Add(strPointer, listPointer);
                }
            }

            //more than 2
            List<Intersection> listIntersections = new List<Intersection>();
            foreach (KeyValuePair<string, List<string>> pair in dicPointers)
            {
                //more than 2 is intersection
                if (pair.Value.Count > 2)
                {
                    Intersection interSection = new Intersection();
                    interSection.X = Convert.ToInt32(pair.Key.Split(',')[0]);
                    interSection.Y = Convert.ToInt32(pair.Key.Split(',')[1]);

                    foreach (string point in pair.Value)
                    {
                        Chunk cnk = new Chunk();
                        cnk.X1 = interSection.X;
                        cnk.X2 = Convert.ToInt32(point.Split(',')[0]);
                        cnk.Y1 = interSection.Y;
                        cnk.Y2 = Convert.ToInt32(point.Split(',')[1]);

                        cnk = FindChunk(cnk.X1,cnk.Y1, cnk.X2, cnk.Y2);
                        
                        if (interSection.Chunks == null)
                            interSection.Chunks = new List<Chunk>();
                        
                        interSection.Chunks.Add(cnk);
                    }

                    interSection.RoadCount = pair.Value.Count;
                    listIntersections.Add(interSection);
                }
            }

            return listIntersections;
        }

        public static List<Chunk> GetRoute(int MapID)
        {
            //1. Convert Array to Dictionary 
            ConvertMap(MapID);
            ConvertMapOppositeWay(MapID);

            //2. Generate Random Road
            // From 1 to maximum between 20 and 50 chunks
            Random rd=new Random();

            List<Chunk> listChunk = new List<Chunk>();
            int Maximum = rd.Next(20,50);

            //Define Way Random
            Dictionary<string, List<string>> DicChunks;
            //if (IsOneway)
            //{
            //    DicChunks = dicChunks;
            //    IsOneway = false;
            //}
            //else
            //{
            //    DicChunks = dicChunksOpposite;
            //    IsOneway = true;
            //}

            DicChunks = dicChunks;
                IsOneway = false;

            //string FirstPointer;
            string EndPointer="";
            
            //3. Generating Data
            for (int i = 0; i < Maximum; i++)
            {
                if (EndPointer == "")
                {
                    //First Pointer - Random
                    EndPointer = DicChunks.Keys.ElementAt(rd.Next(DicChunks.Count));
                }
                //End Pointer -> Finding Random Pointer
                if (DicChunks.ContainsKey(EndPointer))
                {
                    Chunk c = new Chunk();
                    // Start Point
                    c.X1 = Convert.ToInt32(EndPointer.Split(',')[0]);
                    c.Y1 = Convert.ToInt32(EndPointer.Split(',')[1]);

                    //Random - > EndPoint
                    List<string> Pointers = DicChunks[EndPointer];
                    rd=new Random();
                    int NewPointer = rd.Next(Pointers.Count);
                    EndPointer = Pointers[NewPointer];

                    // End Point
                    c.X2 = Convert.ToInt32(EndPointer.Split(',')[0]);
                    c.Y2 = Convert.ToInt32(EndPointer.Split(',')[1]);
                    
                    c = SetChunkData(c, IsOneway);

                    listChunk.Add(c);
                }
                else//last road
                    break;
            }

            return listChunk;
        }
        #endregion

        #region Private Functions
        private static Chunk FindChunk(int x1, int y1, int x2, int y2)
        {
            Chunk cnk = new Chunk();
            cnk.X1 = x1;
            cnk.X2 = x2;
            cnk.Y1 = y1;
            cnk.Y2 = y2;

            cnk = SetChunkData(cnk);
            if (cnk != null)
                return cnk;
            else
            {
                cnk = new Chunk();
                cnk.X1 = x2;
                cnk.X2 = x1;
                cnk.Y1 = y2;
                cnk.Y2 = y1;
                return SetChunkData(cnk, false);
            }
        }

        private static Chunk SetChunkData(Chunk cnk)
        {
            return SetChunkData(cnk,false);
        }

        private static Chunk SetChunkData(Chunk cnk, bool IsReversed)
        {
            if (listChunks == null)
                GetMap(0);

            foreach (Chunk c in listChunks)
            {
                if (!IsReversed)//Reverse
                {
                    if (c.X1 == cnk.X1 && c.X2 == cnk.X2 && c.Y1 == cnk.Y1 && c.Y2 == cnk.Y2)
                    {
                        cnk.ID = c.ID;
                        cnk.SpeedLimit = c.SpeedLimit;
                        return c;
                    }
                }
                else
                {
                    if (c.X1 == cnk.X2 && c.X2 == cnk.X1 && c.Y1 == cnk.Y2 && c.Y2 == cnk.Y1)
                    {
                        cnk.ID = c.ID;
                        cnk.SpeedLimit = c.SpeedLimit;
                        return c;
                    }
                }
            }

            return null;
        }

        private static void ConvertMap(int MapID)
        {
            if (dicChunks == null)
            {
                List<Chunk> listChunks = GetMap(MapID);
                dicChunks = new Dictionary<string, List<string>>();
                foreach (Chunk cnk in listChunks)
                {
                    //if key exist
                    if (dicChunks.ContainsKey(cnk.X1.ToString() + "," + cnk.Y1.ToString()))
                    {
                        List<string> YPointers = dicChunks[cnk.X1.ToString() + "," + cnk.Y1.ToString()];
                        YPointers.Add(cnk.X2.ToString() + "," + cnk.Y2.ToString());
                        dicChunks[cnk.X1.ToString() + "," + cnk.Y1.ToString()] = YPointers;
                    }
                    else
                    {
                        List<string> YPointers = new List<string>();
                        YPointers.Add(cnk.X2.ToString() + "," + cnk.Y2.ToString());
                        dicChunks.Add(cnk.X1.ToString() + "," + cnk.Y1.ToString(), YPointers);
                    }
                }
            }
        }

        private static void ConvertMapOppositeWay(int MapID)
        {
            if (dicChunksOpposite == null)
            {
                List<Chunk> listChunks = GetMap(MapID);
                dicChunksOpposite = new Dictionary<string, List<string>>();
                foreach (Chunk cnk in listChunks)
                {
                    //if key exist
                    if (dicChunksOpposite.ContainsKey(cnk.X2.ToString() + "," + cnk.Y2.ToString()))
                    {
                        List<string> YPointers = dicChunksOpposite[cnk.X2.ToString() + "," + cnk.Y2.ToString()];
                        YPointers.Add(cnk.X1.ToString() + "," + cnk.Y1.ToString());
                        dicChunksOpposite[cnk.X2.ToString() + "," + cnk.Y2.ToString()] = YPointers;
                    }
                    else
                    {
                        List<string> YPointers = new List<string>();
                        YPointers.Add(cnk.X1.ToString() + "," + cnk.Y1.ToString());
                        dicChunksOpposite.Add(cnk.X2.ToString() + "," + cnk.Y2.ToString(), YPointers);
                    }
                }
            }
        }
        #endregion
    }
}
