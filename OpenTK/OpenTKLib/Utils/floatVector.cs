// Pogramming by
//     Douglas Andrade ( http://www.cmsoft.com.br, email: cmsoft@cmsoft.com.br)
//               Implementation of most of the functionality
//     Edgar Maass: (email: maass@logisel.de)
//               Code adaption, changed to user control
//
//Software used: 
//    OpenGL : http://www.opengl.org
//    OpenTK : http://www.opentk.com
//
// DISCLAIMER: Users rely upon this software at their own risk, and assume the responsibility for the results. Should this software or program prove defective, 
// users assume the cost of all losses, including, but not limited to, any necessary servicing, repair or correction. In no event shall the developers or any person 
// be liable for any loss, expense or damage, of any type or nature arising out of the use of, or inability to use this software or program, including, but not
// limited to, claims, suits or causes of action involving alleged infringement of copyrights, patents, trademarks, trade secrets, or unfair competition. 
//

using System;

namespace OpenTKLib
{
  public class floatVector : IComparable<floatVector>
  {
    public float x;
    public float y;
    public float z;

    public floatVector()
    {
      this.x = 0.0f;
      this.y = 0.0f;
      this.z = 0.0f;
    }

    public floatVector(float xComponent, float yComponent, float zComponent)
    {
      this.x = xComponent;
      this.y = yComponent;
      this.z = zComponent;
    }

    public floatVector(floatVector v)
    {
      this.x = v.x;
      this.y = v.y;
      this.z = v.z;
    }

    public static floatVector operator +(floatVector v1, floatVector v2)
    {
      return new floatVector()
      {
        x = v1.x + v2.x,
        y = v1.y + v2.y,
        z = v1.z + v2.z
      };
    }

    public static floatVector operator -(floatVector v1, floatVector v2)
    {
      return new floatVector()
      {
        x = v1.x - v2.x,
        y = v1.y - v2.y,
        z = v1.z - v2.z
      };
    }

    public static floatVector operator *(float num, floatVector v)
    {
      return new floatVector()
      {
        x = v.x * num,
        y = v.y * num,
        z = v.z * num
      };
    }

    public static floatVector operator *(floatVector v, float num)
    {
      return new floatVector()
      {
        x = v.x * num,
        y = v.y * num,
        z = v.z * num
      };
    }

    public static floatVector operator /(float num, floatVector v)
    {
      return new floatVector()
      {
        x = v.x / num,
        y = v.y / num,
        z = v.z / num
      };
    }

    public static floatVector operator /(floatVector v, float num)
    {
      return new floatVector()
      {
        x = v.x / num,
        y = v.y / num,
        z = v.z / num
      };
    }

    public override string ToString()
    {
      return "(" + this.x.ToString() + ";" + this.y.ToString() + ";" + this.z.ToString() + ")";
    }

    public int CompareTo(floatVector v)
    {
      return (double) this.x == (double) v.x && (double) this.y == (double) v.y && (double) this.z == (double) v.z ? 0 : 1;
    }

    public static float DotProduct(floatVector v1, floatVector v2)
    {
      return (float) ((double) v1.x * (double) v2.x + (double) v1.y * (double) v2.y + (double) v1.z * (double) v2.z);
    }

    public static floatVector CrossProduct(floatVector v1, floatVector v2)
    {
      return new floatVector()
      {
        x = (float) ((double) v1.y * (double) v2.z - (double) v2.y * (double) v1.z),
        y = (float) (-(double) v1.x * (double) v2.z + (double) v2.x * (double) v1.z),
        z = (float) ((double) v1.x * (double) v2.y - (double) v2.x * (double) v1.y)
      };
    }

    public float norm()
    {
      return (float) Math.Sqrt((double) floatVector.DotProduct(this, this));
    }

    public void normalize()
    {
      float num = 1f / this.norm();
      this.x *= num;
      this.y *= num;
      this.z *= num;
    }
  }
}
