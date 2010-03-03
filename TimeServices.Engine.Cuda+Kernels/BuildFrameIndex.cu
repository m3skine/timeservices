__global__ void BuildFrameIndex(float *a, float *b, float *c)
{
	int index = threadIdx.x;
	c[index] = a[index] + b[index];
}
