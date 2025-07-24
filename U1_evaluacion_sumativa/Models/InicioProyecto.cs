﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace U1_evaluacion_sumativa.Models;

public partial class InicioProyecto
{
    public int Id { get; set; }

    public string EntidadInvolucrada { get; set; } = null!;

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFinalizacion { get; set; }

    public int? Estado { get; set; }

    public string? Observaciones { get; set; }

    public int ProyectoId { get; set; }

    [ValidateNever] 
    public virtual ICollection<DireccionamientoIp> DireccionamientoIps { get; set; } = new List<DireccionamientoIp>();

    [ValidateNever] 
    public virtual Proyecto Proyecto { get; set; } = null!;

    [ValidateNever] 
    public virtual ICollection<Trabajadore> Trabajadores { get; set; } = new List<Trabajadore>();
}
