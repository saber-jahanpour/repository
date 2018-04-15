using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace WinTasom
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;

		tasom2dclusterf td2=null;
		//Thread animation=null;
		//static int REFRESH=10;
		//Font myfont;
		int maxneuron=1000,hereiw=0,kmul=30,once=1,cof=5;
		double DWU=.7,DWD=.06;
		bool drawnow=false;
		public int noneuron=30;
		int idim=2,xs,ys;
		double[] x;
		public int inum=25000;
		public double[,] w; 
		double[] delta,sigma,now;

		private Graphics offscreen=null;
		private Bitmap offbitmap=null;
		private Pen pen1,pen2;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;


		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			noneuron=2; 	inum=noneuron*kmul;
			x=new double[idim];
			w=new double[maxneuron,idim];	delta=new double[maxneuron];
			sigma=new double[maxneuron];	now=new double[maxneuron];
			for (int i=0;i<maxneuron;++i)	now[i]=0;
			

		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//base.OnPaintBackground (pevent);
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			
			offbitmap=new Bitmap(this.ClientRectangle.Width,this.ClientRectangle.Height);
			offscreen=Graphics.FromImage(offbitmap);
			pen1=new Pen(new SolidBrush(Color.White),1F);
			pen2=new Pen(new SolidBrush(Color.Red),1F);		
			xs=this.ClientRectangle.Width/2; ys=this.ClientRectangle.Height/2;
			offscreen.Clear(Color.BlueViolet);
			Invalidate();
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			// 
			// timer1
			// 
			this.timer1.Interval = 50;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem3});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2});
			this.menuItem1.Text = "File";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "Exit";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem4,
																					  this.menuItem5});
			this.menuItem3.Text = "Cluster";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 0;
			this.menuItem4.Text = "Gaussian";
			this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 1;
			this.menuItem5.Text = "Uniform";
			this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "Form1";
			this.Resize += new System.EventHandler(this.Form1_Resize);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			
			if(drawnow)
			{
		
				removeunused();	
				redisneurons();
				trainagain();
				myoffscreen();
			}
			Invalidate();
		}

		private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
				e.Graphics.DrawImage(offbitmap ,0,0,this.ClientRectangle.Width,this.ClientRectangle.Height);
		}

		
		public double getweight(int i)
		{
			double w1=w[i,0];
			return w1;
		}

		public void trainagain() 
		{

			if(noneuron<2) noneuron=2;
			if(noneuron>=maxneuron) noneuron=maxneuron-1;
			inum=noneuron*kmul;
			//td2=new tasom2dcluster(non2,.05,.05,.01,1,20,idim);
			td2.onlyparamdef(ref w,ref delta,ref sigma,noneuron);
			for (int i=0;i<inum;++i) 
			{
				if(hereiw==0)
					for(int j=0;j<idim;++j) 
						x[j]=td2.nextuf1(5.0);
				else
					for(int j=0;j<idim;++j) 
						x[j]=td2.nextgu1(1.0);
				td2.setx(ref x);
				td2.learning2d();
			}
		} //train process		

		public void traincluster(int inputwhich) 
		{

			drawnow=false;
			hereiw=inputwhich;
			noneuron=2;
			if(td2==null)
				td2=new tasom2dclusterf(noneuron,.05,.05,.01,50,50,idim);
			else td2.setnon(noneuron);
			td2.weightrandom(ref w);
			td2.otherparam(ref delta, ref sigma, ref now);
			for (int i=0;i<inum;++i) 
			{
				if(hereiw==0)
					for(int j=0;j<idim;++j) 
						x[j]=td2.nextuf1(5.0);
				else
					for(int j=0;j<idim;++j) 
						x[j]=td2.nextgu1(1.0);
				td2.setx( ref x);
				td2.learning2d();
			}
			drawnow=true;

		} //train process	

		//add or delete neurons 
		void removeunused()
		{
	
			//check for unnused neurons
			int n1=0,n2;
			while((n1<noneuron) && (noneuron>2))
			{
      
           
				if (now[n1]==0)
				{
                          
					for(n2=n1;n2<(noneuron-1);++n2)
					{
						w[n2,0]=w[n2+1,0]; w[n2,1]=w[n2+1,1];
						delta[n2]=delta[n2+1];
						sigma[n2]=sigma[n2+1];
                  
						now[n2]=now[n2+1];
                  
					}
					noneuron--;
             
				}   //if now==0
				n1++; 
			} //  loop k1 first
		}//end of addordelete neuron

		//add neurons or delete neurons
		public int getnon()
		{
			return (noneuron);
		}

		void redisneurons()
		{

			double[,] wp=new double[maxneuron,idim];
			double[] sigma1=new double[maxneuron];
			double[] delta1=new double[maxneuron];
			double[] now1=new double[maxneuron];
			int[] index=new int[maxneuron];
			int n1=0,n2;
			double tempd;
			while(n1<(noneuron-1))
			{

				// for close contour k1=non is considered
     
				n2=n1+1; 
				if (n2>=noneuron)
				{
					w[n2,0]=w[0,0]; w[n2,1]=w[0,1];
					now[n2]=now[0];
					sigma[n2]=sigma[0]; delta[n2]=delta[0];
				}  //if n2>noneuron
            
		  
		  
				tempd=(w[n1,0]-w[n2,0])*(w[n1,0]-w[n2,0]);
				tempd+=(w[n1,1]-w[n2,1])*(w[n1,1]-w[n2,1]);
				tempd=Math.Sqrt(tempd);
				//if (tempd>DWU) cflag=1;
				index[n1]=0;
				//if(now[n1]>=(coef*kmul))
				//index[n1]=1;
				if ((tempd>DWU) && (now[n1]!=0) && (now[n2]!=0))
				{index[n1]=1; }
				else
					if ((tempd<DWD)  && (now[n1]!=0) && (now[n2]!=0))

				{index[n1]=-1;}
       	    
				++n1;
			}// while n1 loop
     
			index[noneuron-1]=0;
			int n21,n11;
			int noneuron1=noneuron;
			n2=0;
    
			for (n1=0;n1<noneuron;++n1)
			{
				if (index[n1]==0)
				{
					wp[n2,0]=w[n1,0]; wp[n2,1]=w[n1,1]; 
					sigma1[n2]=sigma[n1]; 
					delta1[n2]=delta[n1];
					now1[n2]=now[n1];
					++n2;}

				else
					if(index[n1]==1)
				{
					wp[n2,0]=w[n1,0]; wp[n2,1]=w[n1,1]; 
					sigma1[n2]=sigma[n1]; 
					delta1[n2]=delta[n1];
					now1[n2]=now[n1];
		
					n21=n2+1; n11=n1+1;
					wp[n21,0]=.5*(w[n1,0]+w[n11,0]); wp[n21,1]=.5*(w[n1,1]+w[n11,1]);
					sigma1[n21]=.5*(sigma[n1]+sigma[n11]);
					delta1[n21]=.5*(delta[n1]+delta[n11]);
					now1[n21]=1;

					n2=n2+2;
					noneuron1++;}

				else
					if (index[n1]==-1)
				{

					n21=n2+1; n11=n1+1;

					wp[n2,0]=.5*(w[n1,0]+w[n11,0]); wp[n2,1]=.5*(w[n1,1]+w[n11,1]);
					sigma1[n2]=.5*(sigma[n1]+sigma[n11]);
					delta1[n2]=.5*(delta[n1]+delta[n11]);
					now1[n2]=1;
		
					n2=n21; index[n11]=-2; 
					--noneuron1;}

				else
					if (index[n1]==-2)
					n2=n2;

			}// loop of n1        

			noneuron=n2;//ode1;
			for(int n=0;n<noneuron;++n)
			{
				sigma[n]=sigma1[n];
				delta[n]=delta1[n];
				w[n,0]=wp[n,0];
				w[n,1]=wp[n,1];
				now[n]=now1[n];
			}
	
		}// end of redis

		
		
		
		
		
		
		
		
		private void myoffscreen()
		{
			int x1,x2,y1,y2,i;
			offscreen.Clear(Color.BlueViolet);		
			//offscreen.FillRectangle(new SolidBrush(Color.Blue),0,0,this.Width,this.Height);  // clear buffer
			//offscreen.setColor(Color.blue);
			//offscreen.drawString("no. neurons="+noneuron,xs,ys);
			//offscreen.setColor(Color.white);
			if(drawnow)
				if(hereiw==0)
					for (i=0;i<(noneuron-1);++i)
					{
						//x1=(int)((this.Width-1)*.2*w[i,0]);
						//y1=(int)((this.Height-1)*.2*w[i,1]);
						//x2=(int)((this.Width-1)*.2*w[i+1,0]);
						//y2=(int)((this.Height-1)*.2*w[i+1,1]);
						x1=xs/2+(int)(xs*.2*w[i,0]);
						y1=ys/2+(int)(ys*.2*w[i,1]);
						x2=xs/2+(int)(xs*.2*w[i+1,0]);
						y2=ys/2+(int)(ys*.2*w[i+1,1]);
						offscreen.DrawLine(pen1,new Point(x1,y1),new Point(x2,y2));
					}
				else
					for (i=0;i<(noneuron-1);++i)
					{
						x1=(int)((xs/4*w[i,0]+xs));
						y1=(int)((ys/4*w[i,1]+ys));
						x2=(int)((xs/4*w[i+1,0]+xs));
						y2=(int)((ys/4*w[i+1,1]+ys));
						offscreen.DrawLine(pen1,new Point(x1,y1),new Point(x2,y2));

					}	

		}

		

		private void Form1_Resize(object sender, System.EventArgs e)
		{
			offbitmap=new Bitmap(this.ClientRectangle.Width,this.ClientRectangle.Height);
			offscreen=Graphics.FromImage(offbitmap);
			xs=this.ClientRectangle.Width/2; ys=this.ClientRectangle.Height/2;
		}

		private void menuItem4_Click(object sender, System.EventArgs e)
		{
			traincluster(1);
			timer1.Enabled=true;
		}

		private void menuItem5_Click(object sender, System.EventArgs e)
		{
			traincluster(0);
			timer1.Enabled=true;
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			Close();
		}
		
	}
}
