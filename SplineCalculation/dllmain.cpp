#include "pch.h"
#include "mkl.h"

constexpr int DIMENSION = 1;
constexpr int POW = 3;

double* interval_g;
double* initialX_g;
double* trueY_g;


extern "C" _declspec(dllexport)
void spline(double* initialY, int n, double* interval, double* y, int initialN, double* initialX)
{
	DFTaskPtr task;

	if (dfdNewTask1D(&task, n, interval, DF_UNIFORM_PARTITION, DIMENSION, y, DF_NO_HINT) != DF_STATUS_OK) {
		throw 1;
	}
	
	double border[2] = { 0, 0 };
	double* coef = new double[DIMENSION * (POW + 1) * (n - 1)];
	if (dfdEditPPSpline1D(task, 
						DF_PP_CUBIC, 
						DF_PP_NATURAL, 
						DF_BC_2ND_LEFT_DER | DF_BC_2ND_RIGHT_DER, 
						border, 
						DF_NO_IC, 
						NULL, 
						coef, 
						DF_NO_HINT) != DF_STATUS_OK) {
		throw 2;
	}

	if (dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD) != DF_STATUS_OK) {
		throw 3;
	}

	int rule[1] = { 1 };
	if (dfdInterpolate1D(task, 
						DF_INTERP, 
						DF_METHOD_PP, 
						initialN,  
						initialX, 
						DF_NON_UNIFORM_PARTITION,
						1, 
						rule, 
						NULL, 
						initialY, 
						DF_NO_HINT, 
						NULL) != DF_STATUS_OK) {
		throw 4;
	}

	if (dfDeleteTask(&task) != DF_STATUS_OK) {
		throw 5;
	}

	delete[] coef;
}

void function(MKL_INT* initialN, MKL_INT* splineN, double* y, double* f)
{
	spline(f, *splineN, interval_g, y, *initialN, initialX_g);

	for (int i = 0; i < *initialN; i++) {
		f[i] = f[i] - trueY_g[i];
	}
}

extern "C" _declspec(dllexport)
void calculateSpline(int initialN,
	double* initialX,
	double* initialY,
	int splineN,
	double minResidual,
	int maxIterations,
	double* interval,
	int& iterationsCount,
	int& criteria,
	double& min,
	double* vector,
	int& error,
	double* supportX,
	int supportVectorN,
	double* supportY)
{
	_TRNSP_HANDLE_t task;
	double* y = new double[splineN];
	double* jacoby = new double[splineN * initialN];
	initialX_g = initialX;
	interval_g = interval;
	trueY_g = initialY;
	error = 0;

	try {

		const double eps[6] = { minResidual, minResidual, minResidual, minResidual, minResidual, minResidual };
		double startInterval = 30;

		if (dtrnlsp_init(&task, &splineN, &initialN, y, eps, &maxIterations, &maxIterations, &startInterval) != TR_SUCCESS) {
			throw 11;
		}

		int error[6];
		if (dtrnlsp_check(&task, &splineN, &initialN, y, jacoby, vector, error) != TR_SUCCESS) {
			throw 12;
		}

		MKL_INT request = 0;
		while (true) {
			if (dtrnlsp_solve(&task, vector, jacoby, &request) != TR_SUCCESS) {
				throw 13;
			}

			if (request == 0)
				continue;
			else if (request == 1)
				function(&initialN, &splineN, y, vector);
			else if (request == 2)
			{
				if (djacobi(function, &splineN, &initialN, jacoby, y, &minResidual) != TR_SUCCESS) {
					throw 14;
				}
			}
			else if (request >= -6 && request <= -1) break;
			else throw 15;

			
		}
		int iterationsCount_copy;
		int criteria_copy;
		double min_copy;
		double max_copy;
		if (dtrnlsp_get(&task, &iterationsCount_copy, &criteria_copy, &max_copy, &min_copy) != TR_SUCCESS) {
			throw 16;
		}

		iterationsCount = iterationsCount_copy;
		criteria = criteria_copy;
		min = min_copy;

		if (dtrnlsp_delete(&task) != TR_SUCCESS) {
			throw 17;
		}
		spline(vector, splineN, interval_g, y, initialN, initialX_g);

		spline(supportY, splineN, interval_g, y, supportVectorN, supportX);
		
		delete[] y;
		delete[] jacoby;
	}
	catch (int err) {
		error = err;
		delete[] y;
		delete[] jacoby;
	}
}
