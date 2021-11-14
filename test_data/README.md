# Test Data Details

This is a description of the data sets that were used in the original article. So presented here are some details of the input data sets, and the expected "ideal" output results.

---

*4_point_symmetric_simple_maxima_data.csv*

As the name suggests, this data is a simple dataset that consists of 4 points. The peak is not one of the 4 points. The dataset is 'simple' though due to it being symmetric about a single peak at the X axis and thus represents a simple maxima. This dataset though does show that the algorithm can handle data that has negative X values.

*4_point_symmetric_simple_maxima_data.expected_output.csv*

The single peak at the X axis can be determined by plotting the points and interrogating the Y value at X = 0. 

---

*3_point_asymmetric_data.csv*

This dataset is the same as the previous 4 point dataset, except that the first point is removed. It is intuitive that 2 data points cannot yield an extremum since that would only be a straight line. Thus 3 points represents the minumum number of data point that could yield any min or max when plotted out. So this is a stress test since it's the bare minimum input required to solve for an extrema. 

*3_point_asymmetric_data.expected_output.csv*

Since the first negative X point was removed, we would expect the extrema to shift to the positive direction compared to the 4 point symmetric result. Also, we should expect the computed direct result to be a bit less accurate given that only 3 input points are provided.

---

*7_point_trajectory_data.csv*

This data was hand-calculated based upon the standard equations for trajectories. A 45 degree shot at 1500 ft/sec was used for this data set.

*7_point_trajectory_data.expected_output.csv*

The Hmax (e.g. Y value) for this trajectory is calculated to be approximately 17,468.94 ft and at that height the X value is calculated as 34,937.89 ft.

---

*15_point_underdamped_second_order_data.csv*
*15_point_underdamped_second_order_data.expected_output.csv*

---

*8_point_2_root_data.csv*
*8_point_2_root_data.output_expected.csv*

---

*8_point_critically_damped_second_order_data.csv*
*8_point_critically_damped_second_order_data.expected_output.csv*