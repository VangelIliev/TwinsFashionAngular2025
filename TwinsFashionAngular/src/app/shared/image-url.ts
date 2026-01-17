const CLOUDINARY_UPLOAD_SEGMENT = '/upload/';

export interface CloudinaryTransformOptions {
  width: number;
  aspectRatio?: string; // e.g. "3:4"
  crop?: 'fill' | 'fit' | 'scale';
  gravity?: 'auto' | 'center';
}

function isLikelyCloudinaryUrl(url: string): boolean {
  return typeof url === 'string' && url.includes('res.cloudinary.com') && url.includes(CLOUDINARY_UPLOAD_SEGMENT);
}

function buildTransformString(opts: CloudinaryTransformOptions): string {
  const parts: string[] = ['f_auto', 'q_auto', 'dpr_auto'];

  if (opts.crop === 'fit') {
    parts.push('c_fit');
  } else if (opts.crop === 'scale') {
    parts.push('c_scale');
  } else {
    parts.push('c_fill');
  }

  if (opts.gravity === 'center') {
    parts.push('g_center');
  } else {
    parts.push('g_auto');
  }

  if (opts.aspectRatio) {
    parts.push(`ar_${opts.aspectRatio}`);
  }

  parts.push(`w_${opts.width}`);
  return parts.join(',');
}

export function cloudinaryTransformUrl(url: string, opts: CloudinaryTransformOptions): string {
  if (!url || !isLikelyCloudinaryUrl(url)) {
    return url;
  }

  const transform = buildTransformString(opts);
  const idx = url.indexOf(CLOUDINARY_UPLOAD_SEGMENT);
  const prefix = url.slice(0, idx + CLOUDINARY_UPLOAD_SEGMENT.length);
  const rest = url.slice(idx + CLOUDINARY_UPLOAD_SEGMENT.length);
  return `${prefix}${transform}/${rest}`;
}

export function cloudinarySrcset(
  url: string,
  widths: number[],
  opts: Omit<CloudinaryTransformOptions, 'width'>
): string | null {
  if (!url || !isLikelyCloudinaryUrl(url)) {
    return null;
  }

  const unique = Array.from(new Set(widths)).filter(w => Number.isFinite(w) && w > 0).sort((a, b) => a - b);
  if (unique.length === 0) {
    return null;
  }

  return unique
    .map(w => `${cloudinaryTransformUrl(url, { ...opts, width: w })} ${w}w`)
    .join(', ');
}


