﻿namespace Widtop.Hid
{
    public static class DischargeCurve
    {
        public class Discharge
        {
            public int Minutes { get; }
            public double Volts { get; }
            public double mWh { get; }

            public Discharge(int minutes, double volts, double mwh)
            {
                Minutes = minutes;
                Volts = volts;
                mWh = mwh;
            }
        }

        public static readonly Discharge[] Values =
        {
            new Discharge(0, 4.186, 1.34),
            new Discharge(1, 4.172, 2.73),
            new Discharge(2, 4.166, 4.13),
            new Discharge(3, 4.163, 5.52),
            new Discharge(4, 4.160, 6.90),
            new Discharge(5, 4.157, 8.28),
            new Discharge(6, 4.156, 9.67),
            new Discharge(7, 4.153, 11.06),
            new Discharge(8, 4.152, 12.43),
            new Discharge(9, 4.150, 13.80),
            new Discharge(10, 4.148, 15.17),
            new Discharge(11, 4.146, 16.56),
            new Discharge(12, 4.144, 17.93),
            new Discharge(13, 4.143, 19.32),
            new Discharge(14, 4.141, 20.70),
            new Discharge(15, 4.139, 22.07),
            new Discharge(16, 4.138, 23.46),
            new Discharge(17, 4.136, 24.83),
            new Discharge(18, 4.135, 26.21),
            new Discharge(19, 4.133, 27.58),
            new Discharge(20, 4.131, 28.95),
            new Discharge(21, 4.130, 30.33),
            new Discharge(22, 4.128, 31.71),
            new Discharge(23, 4.127, 33.08),
            new Discharge(24, 4.125, 34.45),
            new Discharge(25, 4.124, 35.84),
            new Discharge(26, 4.122, 37.21),
            new Discharge(27, 4.121, 38.57),
            new Discharge(28, 4.119, 39.95),
            new Discharge(29, 4.117, 41.33),
            new Discharge(30, 4.116, 42.69),
            new Discharge(31, 4.115, 44.06),
            new Discharge(32, 4.113, 45.43),
            new Discharge(33, 4.112, 46.81),
            new Discharge(34, 4.110, 48.18),
            new Discharge(35, 4.109, 49.55),
            new Discharge(36, 4.108, 50.91),
            new Discharge(37, 4.106, 52.28),
            new Discharge(38, 4.105, 53.63),
            new Discharge(39, 4.103, 54.99),
            new Discharge(40, 4.102, 56.36),
            new Discharge(41, 4.100, 57.73),
            new Discharge(42, 4.099, 59.07),
            new Discharge(43, 4.098, 60.44),
            new Discharge(44, 4.096, 61.81),
            new Discharge(45, 4.095, 63.18),
            new Discharge(46, 4.094, 64.55),
            new Discharge(47, 4.092, 65.90),
            new Discharge(48, 4.091, 67.24),
            new Discharge(49, 4.090, 68.61),
            new Discharge(50, 4.088, 69.96),
            new Discharge(51, 4.087, 71.32),
            new Discharge(52, 4.086, 72.68),
            new Discharge(53, 4.084, 74.05),
            new Discharge(54, 4.082, 75.41),
            new Discharge(55, 4.081, 76.76),
            new Discharge(56, 4.080, 78.12),
            new Discharge(57, 4.079, 79.49),
            new Discharge(58, 4.077, 80.86),
            new Discharge(59, 4.076, 82.23),
            new Discharge(60, 4.075, 83.59),
            new Discharge(61, 4.074, 84.93),
            new Discharge(62, 4.073, 86.29),
            new Discharge(63, 4.071, 87.65),
            new Discharge(64, 4.070, 88.99),
            new Discharge(65, 4.069, 90.35),
            new Discharge(66, 4.067, 91.69),
            new Discharge(67, 4.066, 93.04),
            new Discharge(68, 4.065, 94.40),
            new Discharge(69, 4.064, 95.75),
            new Discharge(70, 4.062, 97.12),
            new Discharge(71, 4.061, 98.48),
            new Discharge(72, 4.060, 99.84),
            new Discharge(73, 4.059, 101.19),
            new Discharge(74, 4.057, 102.54),
            new Discharge(75, 4.056, 103.87),
            new Discharge(76, 4.055, 105.21),
            new Discharge(77, 4.054, 106.55),
            new Discharge(78, 4.052, 107.88),
            new Discharge(79, 4.051, 109.22),
            new Discharge(80, 4.050, 110.56),
            new Discharge(81, 4.049, 111.91),
            new Discharge(82, 4.048, 113.25),
            new Discharge(83, 4.046, 114.58),
            new Discharge(84, 4.045, 115.91),
            new Discharge(85, 4.044, 117.27),
            new Discharge(86, 4.043, 118.61),
            new Discharge(87, 4.042, 119.96),
            new Discharge(88, 4.040, 121.31),
            new Discharge(89, 4.039, 122.66),
            new Discharge(90, 4.038, 124.01),
            new Discharge(91, 4.036, 125.34),
            new Discharge(92, 4.036, 126.69),
            new Discharge(93, 4.034, 128.03),
            new Discharge(94, 4.033, 129.37),
            new Discharge(95, 4.032, 130.70),
            new Discharge(96, 4.031, 132.03),
            new Discharge(97, 4.030, 133.38),
            new Discharge(98, 4.028, 134.70),
            new Discharge(99, 4.027, 136.05),
            new Discharge(100, 4.026, 137.38),
            new Discharge(101, 4.025, 138.73),
            new Discharge(102, 4.024, 140.08),
            new Discharge(103, 4.022, 141.42),
            new Discharge(104, 4.021, 142.76),
            new Discharge(105, 4.021, 144.09),
            new Discharge(106, 4.019, 145.44),
            new Discharge(107, 4.018, 146.76),
            new Discharge(108, 4.017, 148.10),
            new Discharge(109, 4.016, 149.45),
            new Discharge(110, 4.015, 150.79),
            new Discharge(111, 4.013, 152.12),
            new Discharge(112, 4.012, 153.46),
            new Discharge(113, 4.011, 154.81),
            new Discharge(114, 4.010, 156.14),
            new Discharge(115, 4.009, 157.48),
            new Discharge(116, 4.008, 158.79),
            new Discharge(117, 4.007, 160.14),
            new Discharge(118, 4.005, 161.47),
            new Discharge(119, 4.004, 162.81),
            new Discharge(120, 4.003, 164.14),
            new Discharge(121, 4.002, 165.47),
            new Discharge(122, 4.001, 166.80),
            new Discharge(123, 4.000, 168.11),
            new Discharge(124, 3.999, 169.45),
            new Discharge(125, 3.998, 170.79),
            new Discharge(126, 3.997, 172.13),
            new Discharge(127, 3.996, 173.47),
            new Discharge(128, 3.995, 174.81),
            new Discharge(129, 3.993, 176.14),
            new Discharge(130, 3.993, 177.47),
            new Discharge(131, 3.991, 178.80),
            new Discharge(132, 3.991, 180.14),
            new Discharge(133, 3.989, 181.45),
            new Discharge(134, 3.988, 182.79),
            new Discharge(135, 3.987, 184.12),
            new Discharge(136, 3.986, 185.43),
            new Discharge(137, 3.985, 186.76),
            new Discharge(138, 3.984, 188.09),
            new Discharge(139, 3.983, 189.42),
            new Discharge(140, 3.982, 190.73),
            new Discharge(141, 3.981, 192.05),
            new Discharge(142, 3.980, 193.37),
            new Discharge(143, 3.979, 194.70),
            new Discharge(144, 3.978, 196.03),
            new Discharge(145, 3.977, 197.34),
            new Discharge(146, 3.976, 198.67),
            new Discharge(147, 3.975, 199.98),
            new Discharge(148, 3.974, 201.31),
            new Discharge(149, 3.973, 202.62),
            new Discharge(150, 3.972, 203.94),
            new Discharge(151, 3.971, 205.26),
            new Discharge(152, 3.970, 206.59),
            new Discharge(153, 3.969, 207.92),
            new Discharge(154, 3.967, 209.25),
            new Discharge(155, 3.967, 210.58),
            new Discharge(156, 3.966, 211.89),
            new Discharge(157, 3.965, 213.22),
            new Discharge(158, 3.964, 214.55),
            new Discharge(159, 3.962, 215.87),
            new Discharge(160, 3.961, 217.18),
            new Discharge(161, 3.961, 218.51),
            new Discharge(162, 3.960, 219.83),
            new Discharge(163, 3.959, 221.15),
            new Discharge(164, 3.958, 222.47),
            new Discharge(165, 3.957, 223.79),
            new Discharge(166, 3.956, 225.08),
            new Discharge(167, 3.955, 226.39),
            new Discharge(168, 3.954, 227.71),
            new Discharge(169, 3.953, 229.02),
            new Discharge(170, 3.952, 230.34),
            new Discharge(171, 3.951, 231.67),
            new Discharge(172, 3.950, 232.98),
            new Discharge(173, 3.949, 234.28),
            new Discharge(174, 3.948, 235.58),
            new Discharge(175, 3.947, 236.90),
            new Discharge(176, 3.946, 238.21),
            new Discharge(177, 3.945, 239.53),
            new Discharge(178, 3.944, 240.85),
            new Discharge(179, 3.943, 242.15),
            new Discharge(180, 3.942, 243.48),
            new Discharge(181, 3.941, 244.78),
            new Discharge(182, 3.940, 246.10),
            new Discharge(183, 3.939, 247.41),
            new Discharge(184, 3.938, 248.73),
            new Discharge(185, 3.937, 250.02),
            new Discharge(186, 3.936, 251.33),
            new Discharge(187, 3.935, 252.62),
            new Discharge(188, 3.934, 253.94),
            new Discharge(189, 3.933, 255.26),
            new Discharge(190, 3.933, 256.56),
            new Discharge(191, 3.931, 257.85),
            new Discharge(192, 3.931, 259.16),
            new Discharge(193, 3.930, 260.44),
            new Discharge(194, 3.929, 261.73),
            new Discharge(195, 3.928, 263.04),
            new Discharge(196, 3.927, 264.36),
            new Discharge(197, 3.926, 265.67),
            new Discharge(198, 3.925, 266.98),
            new Discharge(199, 3.924, 268.28),
            new Discharge(200, 3.923, 269.58),
            new Discharge(201, 3.922, 270.89),
            new Discharge(202, 3.921, 272.19),
            new Discharge(203, 3.920, 273.49),
            new Discharge(204, 3.919, 274.79),
            new Discharge(205, 3.919, 276.10),
            new Discharge(206, 3.918, 277.39),
            new Discharge(207, 3.917, 278.68),
            new Discharge(208, 3.916, 279.97),
            new Discharge(209, 3.915, 281.25),
            new Discharge(210, 3.914, 282.54),
            new Discharge(211, 3.913, 283.85),
            new Discharge(212, 3.912, 285.14),
            new Discharge(213, 3.911, 286.42),
            new Discharge(214, 3.910, 287.73),
            new Discharge(215, 3.909, 289.03),
            new Discharge(216, 3.908, 290.34),
            new Discharge(217, 3.907, 291.65),
            new Discharge(218, 3.906, 292.95),
            new Discharge(219, 3.905, 294.24),
            new Discharge(220, 3.904, 295.54),
            new Discharge(221, 3.903, 296.82),
            new Discharge(222, 3.902, 298.12),
            new Discharge(223, 3.901, 299.43),
            new Discharge(224, 3.901, 300.73),
            new Discharge(225, 3.899, 302.02),
            new Discharge(226, 3.899, 303.30),
            new Discharge(227, 3.898, 304.61),
            new Discharge(228, 3.897, 305.91),
            new Discharge(229, 3.896, 307.22),
            new Discharge(230, 3.895, 308.53),
            new Discharge(231, 3.894, 309.83),
            new Discharge(232, 3.893, 311.11),
            new Discharge(233, 3.892, 312.41),
            new Discharge(234, 3.891, 313.69),
            new Discharge(235, 3.890, 314.98),
            new Discharge(236, 3.890, 316.29),
            new Discharge(237, 3.889, 317.58),
            new Discharge(238, 3.887, 318.88),
            new Discharge(239, 3.887, 320.17),
            new Discharge(240, 3.886, 321.45),
            new Discharge(241, 3.885, 322.75),
            new Discharge(242, 3.884, 324.04),
            new Discharge(243, 3.883, 325.34),
            new Discharge(244, 3.882, 326.63),
            new Discharge(245, 3.881, 327.93),
            new Discharge(246, 3.880, 329.23),
            new Discharge(247, 3.879, 330.52),
            new Discharge(248, 3.879, 331.81),
            new Discharge(249, 3.877, 333.10),
            new Discharge(250, 3.877, 334.38),
            new Discharge(251, 3.875, 335.68),
            new Discharge(252, 3.874, 336.97),
            new Discharge(253, 3.874, 338.25),
            new Discharge(254, 3.873, 339.53),
            new Discharge(255, 3.872, 340.81),
            new Discharge(256, 3.871, 342.09),
            new Discharge(257, 3.870, 343.37),
            new Discharge(258, 3.870, 344.67),
            new Discharge(259, 3.868, 345.96),
            new Discharge(260, 3.868, 347.24),
            new Discharge(261, 3.867, 348.53),
            new Discharge(262, 3.866, 349.82),
            new Discharge(263, 3.865, 351.11),
            new Discharge(264, 3.865, 352.40),
            new Discharge(265, 3.864, 353.69),
            new Discharge(266, 3.863, 354.98),
            new Discharge(267, 3.862, 356.27),
            new Discharge(268, 3.861, 357.57),
            new Discharge(269, 3.860, 358.86),
            new Discharge(270, 3.859, 360.15),
            new Discharge(271, 3.859, 361.43),
            new Discharge(272, 3.858, 362.70),
            new Discharge(273, 3.857, 363.97),
            new Discharge(274, 3.857, 365.24),
            new Discharge(275, 3.855, 366.51),
            new Discharge(276, 3.855, 367.78),
            new Discharge(277, 3.854, 369.05),
            new Discharge(278, 3.853, 370.34),
            new Discharge(279, 3.852, 371.63),
            new Discharge(280, 3.851, 372.90),
            new Discharge(281, 3.851, 374.19),
            new Discharge(282, 3.850, 375.47),
            new Discharge(283, 3.849, 376.76),
            new Discharge(284, 3.848, 378.02),
            new Discharge(285, 3.848, 379.31),
            new Discharge(286, 3.847, 380.60),
            new Discharge(287, 3.846, 381.88),
            new Discharge(288, 3.846, 383.17),
            new Discharge(289, 3.845, 384.43),
            new Discharge(290, 3.844, 385.70),
            new Discharge(291, 3.843, 386.99),
            new Discharge(292, 3.842, 388.25),
            new Discharge(293, 3.842, 389.54),
            new Discharge(294, 3.841, 390.82),
            new Discharge(295, 3.840, 392.10),
            new Discharge(296, 3.839, 393.38),
            new Discharge(297, 3.839, 394.65),
            new Discharge(298, 3.838, 395.91),
            new Discharge(299, 3.837, 397.20),
            new Discharge(300, 3.837, 398.48),
            new Discharge(301, 3.836, 399.75),
            new Discharge(302, 3.835, 401.04),
            new Discharge(303, 3.835, 402.30),
            new Discharge(304, 3.834, 403.58),
            new Discharge(305, 3.833, 404.84),
            new Discharge(306, 3.833, 406.12),
            new Discharge(307, 3.832, 407.40),
            new Discharge(308, 3.831, 408.69),
            new Discharge(309, 3.831, 409.97),
            new Discharge(310, 3.830, 411.26),
            new Discharge(311, 3.829, 412.54),
            new Discharge(312, 3.829, 413.82),
            new Discharge(313, 3.828, 415.09),
            new Discharge(314, 3.828, 416.36),
            new Discharge(315, 3.827, 417.63),
            new Discharge(316, 3.826, 418.91),
            new Discharge(317, 3.826, 420.18),
            new Discharge(318, 3.825, 421.44),
            new Discharge(319, 3.824, 422.72),
            new Discharge(320, 3.824, 423.97),
            new Discharge(321, 3.823, 425.24),
            new Discharge(322, 3.823, 426.52),
            new Discharge(323, 3.822, 427.79),
            new Discharge(324, 3.821, 429.05),
            new Discharge(325, 3.821, 430.32),
            new Discharge(326, 3.820, 431.59),
            new Discharge(327, 3.819, 432.86),
            new Discharge(328, 3.819, 434.13),
            new Discharge(329, 3.818, 435.39),
            new Discharge(330, 3.818, 436.64),
            new Discharge(331, 3.817, 437.91),
            new Discharge(332, 3.817, 439.18),
            new Discharge(333, 3.816, 440.45),
            new Discharge(334, 3.815, 441.72),
            new Discharge(335, 3.815, 443.00),
            new Discharge(336, 3.814, 444.27),
            new Discharge(337, 3.814, 445.53),
            new Discharge(338, 3.813, 446.78),
            new Discharge(339, 3.813, 448.05),
            new Discharge(340, 3.812, 449.33),
            new Discharge(341, 3.811, 450.60),
            new Discharge(342, 3.811, 451.85),
            new Discharge(343, 3.811, 453.12),
            new Discharge(344, 3.810, 454.39),
            new Discharge(345, 3.809, 455.67),
            new Discharge(346, 3.809, 456.94),
            new Discharge(347, 3.808, 458.20),
            new Discharge(348, 3.808, 459.47),
            new Discharge(349, 3.807, 460.73),
            new Discharge(350, 3.807, 461.99),
            new Discharge(351, 3.806, 463.25),
            new Discharge(352, 3.806, 464.50),
            new Discharge(353, 3.805, 465.78),
            new Discharge(354, 3.805, 467.05),
            new Discharge(355, 3.804, 468.31),
            new Discharge(356, 3.804, 469.57),
            new Discharge(357, 3.803, 470.83),
            new Discharge(358, 3.803, 472.10),
            new Discharge(359, 3.802, 473.37),
            new Discharge(360, 3.801, 474.64),
            new Discharge(361, 3.801, 475.90),
            new Discharge(362, 3.800, 477.17),
            new Discharge(363, 3.800, 478.42),
            new Discharge(364, 3.799, 479.68),
            new Discharge(365, 3.799, 480.95),
            new Discharge(366, 3.798, 482.22),
            new Discharge(367, 3.798, 483.48),
            new Discharge(368, 3.798, 484.75),
            new Discharge(369, 3.797, 486.02),
            new Discharge(370, 3.796, 487.27),
            new Discharge(371, 3.796, 488.52),
            new Discharge(372, 3.796, 489.77),
            new Discharge(373, 3.795, 491.01),
            new Discharge(374, 3.794, 492.28),
            new Discharge(375, 3.794, 493.53),
            new Discharge(376, 3.793, 494.79),
            new Discharge(377, 3.793, 496.06),
            new Discharge(378, 3.793, 497.33),
            new Discharge(379, 3.792, 498.57),
            new Discharge(380, 3.792, 499.84),
            new Discharge(381, 3.791, 501.11),
            new Discharge(382, 3.791, 502.38),
            new Discharge(383, 3.790, 503.64),
            new Discharge(384, 3.790, 504.90),
            new Discharge(385, 3.789, 506.17),
            new Discharge(386, 3.789, 507.44),
            new Discharge(387, 3.789, 508.69),
            new Discharge(388, 3.788, 509.95),
            new Discharge(389, 3.788, 511.22),
            new Discharge(390, 3.787, 512.48),
            new Discharge(391, 3.787, 513.73),
            new Discharge(392, 3.787, 514.98),
            new Discharge(393, 3.786, 516.24),
            new Discharge(394, 3.786, 517.49),
            new Discharge(395, 3.785, 518.76),
            new Discharge(396, 3.784, 520.02),
            new Discharge(397, 3.784, 521.28),
            new Discharge(398, 3.784, 522.54),
            new Discharge(399, 3.783, 523.81),
            new Discharge(400, 3.783, 525.08),
            new Discharge(401, 3.782, 526.33),
            new Discharge(402, 3.782, 527.59),
            new Discharge(403, 3.781, 528.85),
            new Discharge(404, 3.781, 530.12),
            new Discharge(405, 3.781, 531.37),
            new Discharge(406, 3.780, 532.63),
            new Discharge(407, 3.780, 533.88),
            new Discharge(408, 3.780, 535.15),
            new Discharge(409, 3.779, 536.39),
            new Discharge(410, 3.779, 537.65),
            new Discharge(411, 3.778, 538.91),
            new Discharge(412, 3.778, 540.17),
            new Discharge(413, 3.778, 541.44),
            new Discharge(414, 3.777, 542.70),
            new Discharge(415, 3.777, 543.94),
            new Discharge(416, 3.777, 545.21),
            new Discharge(417, 3.776, 546.46),
            new Discharge(418, 3.776, 547.71),
            new Discharge(419, 3.775, 548.95),
            new Discharge(420, 3.775, 550.22),
            new Discharge(421, 3.774, 551.48),
            new Discharge(422, 3.774, 552.74),
            new Discharge(423, 3.774, 554.00),
            new Discharge(424, 3.773, 555.25),
            new Discharge(425, 3.773, 556.50),
            new Discharge(426, 3.773, 557.74),
            new Discharge(427, 3.772, 558.99),
            new Discharge(428, 3.772, 560.24),
            new Discharge(429, 3.771, 561.49),
            new Discharge(430, 3.771, 562.74),
            new Discharge(431, 3.771, 563.99),
            new Discharge(432, 3.770, 565.25),
            new Discharge(433, 3.770, 566.50),
            new Discharge(434, 3.770, 567.76),
            new Discharge(435, 3.769, 569.02),
            new Discharge(436, 3.769, 570.29),
            new Discharge(437, 3.768, 571.54),
            new Discharge(438, 3.768, 572.79),
            new Discharge(439, 3.768, 574.04),
            new Discharge(440, 3.767, 575.30),
            new Discharge(441, 3.767, 576.55),
            new Discharge(442, 3.766, 577.81),
            new Discharge(443, 3.766, 579.06),
            new Discharge(444, 3.766, 580.31),
            new Discharge(445, 3.765, 581.56),
            new Discharge(446, 3.765, 582.82),
            new Discharge(447, 3.765, 584.07),
            new Discharge(448, 3.764, 585.31),
            new Discharge(449, 3.764, 586.56),
            new Discharge(450, 3.764, 587.80),
            new Discharge(451, 3.763, 589.04),
            new Discharge(452, 3.763, 590.28),
            new Discharge(453, 3.763, 591.53),
            new Discharge(454, 3.762, 592.77),
            new Discharge(455, 3.762, 594.02),
            new Discharge(456, 3.762, 595.26),
            new Discharge(457, 3.761, 596.52),
            new Discharge(458, 3.761, 597.78),
            new Discharge(459, 3.761, 599.03),
            new Discharge(460, 3.760, 600.28),
            new Discharge(461, 3.760, 601.54),
            new Discharge(462, 3.759, 602.79),
            new Discharge(463, 3.759, 604.03),
            new Discharge(464, 3.759, 605.27),
            new Discharge(465, 3.758, 606.52),
            new Discharge(466, 3.758, 607.78),
            new Discharge(467, 3.758, 609.04),
            new Discharge(468, 3.757, 610.29),
            new Discharge(469, 3.757, 611.55),
            new Discharge(470, 3.757, 612.79),
            new Discharge(471, 3.756, 614.04),
            new Discharge(472, 3.756, 615.30),
            new Discharge(473, 3.755, 616.55),
            new Discharge(474, 3.755, 617.80),
            new Discharge(475, 3.754, 619.05),
            new Discharge(476, 3.754, 620.31),
            new Discharge(477, 3.754, 621.54),
            new Discharge(478, 3.753, 622.80),
            new Discharge(479, 3.753, 624.05),
            new Discharge(480, 3.752, 625.29),
            new Discharge(481, 3.752, 626.54),
            new Discharge(482, 3.752, 627.77),
            new Discharge(483, 3.751, 629.02),
            new Discharge(484, 3.751, 630.25),
            new Discharge(485, 3.751, 631.50),
            new Discharge(486, 3.750, 632.76),
            new Discharge(487, 3.750, 634.01),
            new Discharge(488, 3.749, 635.26),
            new Discharge(489, 3.749, 636.51),
            new Discharge(490, 3.749, 637.75),
            new Discharge(491, 3.748, 638.98),
            new Discharge(492, 3.748, 640.23),
            new Discharge(493, 3.747, 641.49),
            new Discharge(494, 3.747, 642.73),
            new Discharge(495, 3.746, 643.98),
            new Discharge(496, 3.745, 645.22),
            new Discharge(497, 3.745, 646.46),
            new Discharge(498, 3.744, 647.71),
            new Discharge(499, 3.744, 648.93),
            new Discharge(500, 3.744, 650.19),
            new Discharge(501, 3.743, 651.44),
            new Discharge(502, 3.743, 652.70),
            new Discharge(503, 3.742, 653.95),
            new Discharge(504, 3.742, 655.17),
            new Discharge(505, 3.741, 656.41),
            new Discharge(506, 3.741, 657.66),
            new Discharge(507, 3.740, 658.90),
            new Discharge(508, 3.739, 660.12),
            new Discharge(509, 3.739, 661.37),
            new Discharge(510, 3.739, 662.61),
            new Discharge(511, 3.738, 663.87),
            new Discharge(512, 3.738, 665.10),
            new Discharge(513, 3.737, 666.35),
            new Discharge(514, 3.736, 667.58),
            new Discharge(515, 3.736, 668.81),
            new Discharge(516, 3.736, 670.06),
            new Discharge(517, 3.735, 671.30),
            new Discharge(518, 3.735, 672.54),
            new Discharge(519, 3.734, 673.78),
            new Discharge(520, 3.734, 675.03),
            new Discharge(521, 3.733, 676.28),
            new Discharge(522, 3.733, 677.53),
            new Discharge(523, 3.732, 678.77),
            new Discharge(524, 3.732, 680.01),
            new Discharge(525, 3.731, 681.27),
            new Discharge(526, 3.731, 682.51),
            new Discharge(527, 3.730, 683.76),
            new Discharge(528, 3.730, 685.01),
            new Discharge(529, 3.729, 686.26),
            new Discharge(530, 3.729, 687.49),
            new Discharge(531, 3.728, 688.73),
            new Discharge(532, 3.728, 689.96),
            new Discharge(533, 3.728, 691.20),
            new Discharge(534, 3.727, 692.44),
            new Discharge(535, 3.726, 693.68),
            new Discharge(536, 3.726, 694.92),
            new Discharge(537, 3.725, 696.16),
            new Discharge(538, 3.725, 697.40),
            new Discharge(539, 3.725, 698.65),
            new Discharge(540, 3.724, 699.90),
            new Discharge(541, 3.724, 701.13),
            new Discharge(542, 3.724, 702.37),
            new Discharge(543, 3.723, 703.60),
            new Discharge(544, 3.722, 704.83),
            new Discharge(545, 3.722, 706.07),
            new Discharge(546, 3.722, 707.32),
            new Discharge(547, 3.721, 708.54),
            new Discharge(548, 3.721, 709.77),
            new Discharge(549, 3.720, 711.02),
            new Discharge(550, 3.720, 712.25),
            new Discharge(551, 3.720, 713.50),
            new Discharge(552, 3.719, 714.74),
            new Discharge(553, 3.719, 715.98),
            new Discharge(554, 3.718, 717.22),
            new Discharge(555, 3.718, 718.47),
            new Discharge(556, 3.717, 719.69),
            new Discharge(557, 3.717, 720.91),
            new Discharge(558, 3.717, 722.15),
            new Discharge(559, 3.716, 723.38),
            new Discharge(560, 3.715, 724.62),
            new Discharge(561, 3.715, 725.84),
            new Discharge(562, 3.715, 727.08),
            new Discharge(563, 3.714, 728.32),
            new Discharge(564, 3.714, 729.55),
            new Discharge(565, 3.713, 730.79),
            new Discharge(566, 3.713, 732.03),
            new Discharge(567, 3.712, 733.27),
            new Discharge(568, 3.712, 734.50),
            new Discharge(569, 3.711, 735.74),
            new Discharge(570, 3.711, 736.96),
            new Discharge(571, 3.710, 738.20),
            new Discharge(572, 3.709, 739.44),
            new Discharge(573, 3.709, 740.66),
            new Discharge(574, 3.709, 741.90),
            new Discharge(575, 3.708, 743.13),
            new Discharge(576, 3.707, 744.37),
            new Discharge(577, 3.707, 745.59),
            new Discharge(578, 3.706, 746.82),
            new Discharge(579, 3.706, 748.05),
            new Discharge(580, 3.705, 749.28),
            new Discharge(581, 3.705, 750.52),
            new Discharge(582, 3.704, 751.74),
            new Discharge(583, 3.704, 752.97),
            new Discharge(584, 3.703, 754.21),
            new Discharge(585, 3.702, 755.44),
            new Discharge(586, 3.702, 756.68),
            new Discharge(587, 3.701, 757.90),
            new Discharge(588, 3.701, 759.11),
            new Discharge(589, 3.700, 760.34),
            new Discharge(590, 3.699, 761.57),
            new Discharge(591, 3.699, 762.79),
            new Discharge(592, 3.698, 764.02),
            new Discharge(593, 3.697, 765.25),
            new Discharge(594, 3.697, 766.47),
            new Discharge(595, 3.697, 767.70),
            new Discharge(596, 3.696, 768.91),
            new Discharge(597, 3.695, 770.15),
            new Discharge(598, 3.695, 771.39),
            new Discharge(599, 3.694, 772.62),
            new Discharge(600, 3.693, 773.86),
            new Discharge(601, 3.693, 775.08),
            new Discharge(602, 3.692, 776.31),
            new Discharge(603, 3.692, 777.53),
            new Discharge(604, 3.691, 778.76),
            new Discharge(605, 3.690, 780.00),
            new Discharge(606, 3.690, 781.22),
            new Discharge(607, 3.689, 782.45),
            new Discharge(608, 3.688, 783.68),
            new Discharge(609, 3.688, 784.90),
            new Discharge(610, 3.687, 786.13),
            new Discharge(611, 3.686, 787.36),
            new Discharge(612, 3.686, 788.60),
            new Discharge(613, 3.685, 789.83),
            new Discharge(614, 3.684, 791.05),
            new Discharge(615, 3.683, 792.28),
            new Discharge(616, 3.683, 793.49),
            new Discharge(617, 3.682, 794.70),
            new Discharge(618, 3.681, 795.93),
            new Discharge(619, 3.680, 797.16),
            new Discharge(620, 3.679, 798.37),
            new Discharge(621, 3.678, 799.60),
            new Discharge(622, 3.677, 800.83),
            new Discharge(623, 3.677, 802.06),
            new Discharge(624, 3.676, 803.29),
            new Discharge(625, 3.675, 804.50),
            new Discharge(626, 3.674, 805.73),
            new Discharge(627, 3.673, 806.94),
            new Discharge(628, 3.673, 808.15),
            new Discharge(629, 3.671, 809.38),
            new Discharge(630, 3.671, 810.61),
            new Discharge(631, 3.670, 811.83),
            new Discharge(632, 3.669, 813.05),
            new Discharge(633, 3.669, 814.26),
            new Discharge(634, 3.668, 815.49),
            new Discharge(635, 3.667, 816.71),
            new Discharge(636, 3.667, 817.94),
            new Discharge(637, 3.666, 819.16),
            new Discharge(638, 3.666, 820.38),
            new Discharge(639, 3.665, 821.61),
            new Discharge(640, 3.665, 822.83),
            new Discharge(641, 3.664, 824.06),
            new Discharge(642, 3.663, 825.29),
            new Discharge(643, 3.663, 826.50),
            new Discharge(644, 3.662, 827.72),
            new Discharge(645, 3.662, 828.95),
            new Discharge(646, 3.662, 830.17),
            new Discharge(647, 3.661, 831.40),
            new Discharge(648, 3.661, 832.62),
            new Discharge(649, 3.660, 833.84),
            new Discharge(650, 3.659, 835.06),
            new Discharge(651, 3.659, 836.27),
            new Discharge(652, 3.658, 837.48),
            new Discharge(653, 3.658, 838.70),
            new Discharge(654, 3.657, 839.90),
            new Discharge(655, 3.657, 841.11),
            new Discharge(656, 3.656, 842.34),
            new Discharge(657, 3.655, 843.56),
            new Discharge(658, 3.654, 844.75),
            new Discharge(659, 3.654, 845.97),
            new Discharge(660, 3.653, 847.19),
            new Discharge(661, 3.652, 848.41),
            new Discharge(662, 3.651, 849.61),
            new Discharge(663, 3.650, 850.83),
            new Discharge(664, 3.649, 852.05),
            new Discharge(665, 3.648, 853.27),
            new Discharge(666, 3.647, 854.48),
            new Discharge(667, 3.646, 855.70),
            new Discharge(668, 3.644, 856.90),
            new Discharge(669, 3.643, 858.10),
            new Discharge(670, 3.641, 859.32),
            new Discharge(671, 3.639, 860.54),
            new Discharge(672, 3.637, 861.76),
            new Discharge(673, 3.635, 862.98),
            new Discharge(674, 3.633, 864.18),
            new Discharge(675, 3.631, 865.39),
            new Discharge(676, 3.628, 866.58),
            new Discharge(677, 3.625, 867.78),
            new Discharge(678, 3.622, 868.99),
            new Discharge(679, 3.619, 870.21),
            new Discharge(680, 3.615, 871.43),
            new Discharge(681, 3.612, 872.63),
            new Discharge(682, 3.608, 873.85),
            new Discharge(683, 3.604, 875.05),
            new Discharge(684, 3.600, 876.26),
            new Discharge(685, 3.596, 877.47),
            new Discharge(686, 3.592, 878.67),
            new Discharge(687, 3.588, 879.86),
            new Discharge(688, 3.583, 881.06),
            new Discharge(689, 3.579, 882.26),
            new Discharge(690, 3.574, 883.46),
            new Discharge(691, 3.569, 884.65),
            new Discharge(692, 3.564, 885.84),
            new Discharge(693, 3.559, 887.02),
            new Discharge(694, 3.554, 888.22),
            new Discharge(695, 3.548, 889.40),
            new Discharge(696, 3.543, 890.58),
            new Discharge(697, 3.537, 891.77),
            new Discharge(698, 3.531, 892.95),
            new Discharge(699, 3.525, 894.14),
            new Discharge(700, 3.519, 895.32),
            new Discharge(701, 3.513, 896.49),
            new Discharge(702, 3.506, 897.66),
            new Discharge(703, 3.500, 898.83),
            new Discharge(704, 3.493, 900.01),
            new Discharge(705, 3.486, 901.17),
            new Discharge(706, 3.479, 904.34)
        };
    }

}