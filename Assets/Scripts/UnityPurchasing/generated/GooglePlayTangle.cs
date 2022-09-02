// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("NZwkNTm10XYiYYsMYz2T4yQ0pPHsdIwmegvjoPI8DLfcMocGLBqwAbcvLiG1ciPiiUtqqPACTM96KJe0DewYW5McVKZAekU0nT+QKCb+jCd4fNf+eWzhZQAGId0bfLXIw6DthvG/tb6Mjd/DOSsgyo0YqB5Hpv3hiClJRvoibcNnFLPYZBUV/nnIbcQdnpCfrx2elZ0dnp6fCljDtA6Soa8dnr2vkpmWtRnXGWiSnp6emp+chxs2dUVOSkxGNewJRJhlnhRc37RGUuN5bIYbc/LPPBc6EDD1q4dkujjv2+lYTGLL3JLy0YdQW1lx8MYc41lD6ClgWuDDUw3iwxshyxPoliRNlvIKnZI5snJyFttToWfaZf9e40MPHYiLIaLGiJ2cnp+e");
        private static int[] order = new int[] { 5,2,10,13,8,11,12,10,11,9,12,13,13,13,14 };
        private static int key = 159;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
