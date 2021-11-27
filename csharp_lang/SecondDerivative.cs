/* File: cubic_extrema.c -- Contains FindCubicExtrema() and supporting routines
 that implement the cubic spline extrema algorithm. Given a set of x,y data
 points, determine in the cubic spline sense the relative extrema of the
 function describing the data. -- by Mike J. Courtney
 */

/* Use input x,y data to form tridiagonal matrix and compute second
 derivatives of function in the cubic spline sense. */
BOOL ComputeSecDerivs (
                       uint num_pnts,    // input - number of x & y points 
                       float *x,                 // input - array of x values 
                       float *y,                 // input - array of y values 
                       float *sec_deriv)         // output - array of 2nd derivatives of intervals 
{
    uint   i;
    float ftemp;      // temporary float 
    float *main_diag; // ptr to matrix main diagonal array 
    float *diag;      // ptr to matrix diagonal array */
    float *right;     // ptr to array of right sides of matrix equations 
    main_diag = malloc((num_pnts - 2) * sizeof(float));
    diag = malloc((num_pnts - 1) * sizeof(float));
    right = malloc((num_pnts - 2) * sizeof(float));
    
    /* compute the matrix main and off-diagonal values */
    /* even though the calling program is suppose to have guaranteed that the
     x values are increasing, assert that neither of the diagonal
     differences are zero to avoid a divide by zero condition */
    for (i = 1; i < num_pnts - 1; i++) {
        main_diag[i-1] = 2.0 * (x[i+1] - x[i-1]);
        assert(main_diag[i-1] > 0);
    }
    for (i = 0; i < num_pnts - 1; i++) {
        diag[i] = x[i+1] - x[i];
        assert(diag[i] > 0);
    }
    // compute right hand side of equation
    for (i = 1; i < num_pnts - 1; i++) {
        right[i-1] = 6.0 * ((y[i+1]-y[i])/diag[i]-(y[i]-y[i-1])/diag[i-1]);
    }
    // forward eliminate tridiagonal
    sec_deriv[0] = 0.0;
    sec_deriv[num_pnts - 1] = 0.0;
    for (i = 1; i < num_pnts - 2; i++) {
        ftemp = diag[i] / main_diag[i];
        right[i] -= (right[i-1] * ftemp);
        main_diag[i] -= (diag[i-1] * ftemp);
    }
    // backward substitution to solve for second derivative at each knot
    for (i = num_pnts - 2; i > 0; i--) {
        sec_deriv[i] = (right[i-1] - diag[i-1] * sec_deriv[i+1]) / main_diag[i-1];
    }
    free(main_diag);
    free(diag);
    free(right);
    return SUCCESS;
}
