
/*  Solve for roots x1 and x2 of a quadratic equation of the form
 a * (x * x) + b * x + c = 0 using the following formula x1 = d / a  and
 x2 = c / d, where d = -0.5 * [b + sgn(b) * sqrt(b*b - 4ac)].
 This algorithm is particularly good at yielding accurate results when
 a and/or c are small values.
 */

class QuadraticRoots {
    void solve(
        float a,      // input - coefficient a of quadratic equation 
        float b,      // input - coefficient b of quadratic equation 
        float c,      // input - coefficient c of quadratic equation 
        float *x1,    // output - first root computed 
        float *x2    // output - second root computed 

        ) {
        float d;      // root algorithm variable 
        BOOL  root_stat;  // status of root computations 
        
        d = b * b - 4 * a *c;
        if (d < 0) {
            return FAILURE;
        }
        else {
            d = (float)sqrt((double)d);
            // make the result of sqrt the sign of b
            if (b < 0 ) {
                d = -d;
            }
            d = -0.5 * (b + d);
            // solve for the roots of the quadratic 
            // if both root computations will yield divide by zero ... fahget about it! 
            if ( (a == 0) && (d == 0) ) {
                return FAILURE;
            }
            
            root_stat = SUCCESS;
            // compute first root if denominator a is not zero 
            if (a == 0) {
                root_stat = FAILURE1;
            } else {
                *x1 = d / a;
            }
            // compute second root if denominator d is not zero 
            if (d == 0) {
                root_stat = FAILURE2;
            } else {
                *x2 = c / d;
            }
            return root_stat;
        }
    }
}