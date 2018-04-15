using System;

namespace WinTasom
{

abstract class tasomclassf
{
	protected int non;
	public int idim;
	protected double a2,b2,ap;
	protected double sf,sd;
	protected double[] delta,sigma,sl;
	protected Random myrand=new Random(3);

}

class tasom2dclusterf : tasomclassf 
{

	private double[,] w;
	protected double[] sum1,sum2, now;
	double[] x;   // x is the input 

	public tasom2dclusterf(int non,double a2,double b2,
		double ap, double sf, double sd, int idim)
	{
		
		this.non=non;
		this.idim=idim;
		this.a2=a2;
		this.b2=b2;
		this.ap=ap;
		this.sf=sf;
		this.sd=sd;
		this.sum1=new double [idim]; 
		this.sum2=new double[idim];
		for(int i=0; i<idim; ++i)
		{
			this.sum1[i]=.01+myrand.NextDouble();
			this.sum2[i]=.01+myrand.NextDouble();
		}
	 }
    
	public void setx(ref double[] x)  // set the current input sample x 
	{
		//x.CopyTo(this.x,0);
		this.x=x;
	}


//think about this and weight ????????????????????????
	public void weightrandom (ref double[,] w)
	{
		//w=new double[non];
		this.w=w;
		for (int i=0; i<non; ++i)
			for(int j=0;j<idim;++j)
				this.w[i,j]=myrand.NextDouble();
	}
	//get delta and sigma
	/*public void getds(double delta[], double sigma[]){
	for(int i=0;i<non;++i)
	{delta[i]=this.delta[i];
	 sigma[i]=this.sigma[i];
	}
	}
	*/
	public void otherparam(ref double[] delta,ref double[] sigma, ref double[] now)
	{
		//delta=new double[non];
		//sigma=new double[non];
		this.delta=delta; this.sigma=sigma; this.now=now;
		sl=new double[idim];
		int i;
	
		for (i=0;i<non;++i)
		{
			delta[i]=1.0;
			sigma[i]=non/2.0;
			now[i]=0;
		}

		for (i=0;i<idim;++i)
			sl[i]=1.0;

	}

	public void onlyparamdef(ref double[,] w, ref double[] delta, ref double[] sigma
		, int non)  //only allocates the memory for params
	{
		this.delta=delta;
		this.sigma=sigma;
		this.w=w;
		this.non=non;
		for (int i=0;i<non;++i)
			now[i]=0;
	}

	public void setnon(int noneuron)
	{
		this.non=noneuron;
	}



	void scaleupdate()
	{
		int i;
		for(i=0;i<idim;++i)
		{
				sum1[i]+=ap*(x[i]-sum1[i]);
			sum2[i]+=ap*(x[i]*x[i]-sum2[i]);
			sl[i]=sum2[i]-sum1[i]*sum1[i];
		}
		for(i=1;i<idim;++i)
			sl[0]+=sl[i];
		if (sl[0]<=.0 )
			sl[0]=.001;
		sl[0]=Math.Sqrt(sl[0]);   

	}


	void sigmaupdate(int minindex)
	{
		double dism;
		int j,minms,minps,non1=non-1;
		if (minindex >0 )
			minms=minindex-1;
		else
			minms=1;
   
		if (minindex < non1)
			minps=minindex+1;
		else
			minps=non1-1;

		dism=.0;
		for(j=0;j<idim;++j)
			dism+=(w[minindex,j]-w[minps,j])*(w[minindex,j]-w[minps,j]);
			
		for(j=0;j<idim;++j)
			dism+=(w[minindex,j]-w[minms,j])*(w[minindex,j]-w[minms,j]);
		dism/=2.0;
		scaleupdate();
		sigma[minindex]+=
			b2*(non-(non/(1+dism/(sl[0]*sf)))-sigma[minindex]);	
	}



	public void learning2d()
	{
		double disn,atem;
		double mindis=double.MaxValue;
		
		double[] dist=new double[non];
			int i,j, minindex=0;
	
		for(i=0;i<non;++i)  // find the nearest weight to input x
		{
			dist[i]=.0;
			for(j=0;j<idim;++j)
				dist[i]+=(x[j]-w[i,j])*(x[j]-w[i,j]);
			dist[i]=Math.Sqrt(dist[i]);
			if (dist[i]<mindis)
			{
					mindis=dist[i];
				minindex=i;}
		}

		sigmaupdate(minindex); // update the winner sigma
		++now[minindex];
		for( i=0; i<non;++i)
		{
			disn=Math.Abs(i-minindex);
			if ( sigma[minindex]<=.00001)
				atem=0;
			else
            
				atem=Math.Exp(-.5*disn*disn/(sigma[minindex]*sigma[minindex]));
         
         
			delta[i]+=a2*(1-(1/(1+dist[i]/(sl[0]*sd)))-delta[i]);
			for(j=0;j<idim;++j)      
				w[i,j]+=(atem*delta[i]*(x[j]-w[i,j]));
		} // end of the i loop for weights

	}



	public double nextgu1(double landa)  //return next gaussian 1-D random variable
	{
		double u1,u2,temp;

		while (true) 
		{
			u1=myrand.NextDouble();//Math.random();
			u2=myrand.NextDouble();//Math.random();
			if (u1!=.0) 
			{
				temp=landa*Math.Sqrt(-2*Math.Log(u1));
				return(temp*Math.Cos(2*Math.PI*u2));
				//z2(i)=temp*Math.Sin(2*Math.PI*u2);
			}
		}

	}

	public double nextuf1(double landa)  //return next uniform 1-D random variable
	{
		return(landa*myrand.NextDouble());
		
	}

}

}
